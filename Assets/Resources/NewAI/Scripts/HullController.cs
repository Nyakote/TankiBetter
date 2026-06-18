using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System;
using Unity.AI.Navigation;

public class HullController : MonoBehaviour
{

    float turnTorque = 200000f;
    [SerializeField] float turnDamping = 1000f;
    [SerializeField] float maxAngularSpeed = 3f;
    [SerializeField] float minTurnAngle = 0.5f;
    [SerializeField] float brakePower = 120000f;
    float minTurnPower = 0.8f;

    [Header("References")]
    HullStatsLoader HSL;
    Rigidbody rb;
    TurretControl TR;

    [SerializeField]
    [Header("Wheels")]
    List<WheelCollider> wheels = new List<WheelCollider>();


    [Header("Controllers")]
    int directionOfAcceleraion = 1;
    float currentPower;
    float stoppingDistance = 1f;

    [SerializeField]
    [Header("Hull Stats")]
    public float maxSpeed;
    public float rotationSpeed;
    public float weight;
    public float power;
    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        ReferencesSetterer();
    }

  

    public void SetAgent(NavMeshAgent agent)
    {
        navMeshAgent = agent;
    }

    private void FixedUpdate()
    {
        LimitSpeed();

        if (navMeshAgent == null)
            return;

        SpeedOnAnglesRipper(navMeshAgent);
        AccelerationController(navMeshAgent);
        TurningController(navMeshAgent);
    }


    private void AccelerationController(NavMeshAgent navMeshAgent)
    {
        float distanceToDestination = Vector3.Distance(transform.position, navMeshAgent.destination);

        if (distanceToDestination <= stoppingDistance)
        {
            StopWheels();
            return;
        }

        foreach (WheelCollider wheel in wheels)
        {
            wheel.brakeTorque = 0f;
            wheel.motorTorque = currentPower * directionOfAcceleraion;
        }
    }

    private void TurningController(NavMeshAgent navMeshAgent)
    {
        if (!HasEnoughWheelsOnGround())
            return;

        Vector3 targetDirection = navMeshAgent.steeringTarget - transform.position;
        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude < 0.01f)
            return;

        targetDirection.Normalize();

        float angle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
        float absAngle = Mathf.Abs(angle);

        if (absAngle < minTurnAngle)
            return;

        float turnDirection = Mathf.Sign(angle);
        if (directionOfAcceleraion == -1)
            turnDirection *= -1f;

        float angle01 = Mathf.Clamp01(absAngle / 90f);
        float turnPowerByAngle = Mathf.Lerp(minTurnPower, 1f, Mathf.Sin(angle01 * Mathf.PI * 0.2f));
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed01 = Mathf.Clamp01(flatVelocity.magnitude / maxSpeed);
        float speedTurnMultiplier = Mathf.Lerp(1f, 0.35f, speed01);

        float currentYawSpeed = rb.angularVelocity.y;
        float dampingTorque = -currentYawSpeed * turnDamping;

        float finalTorque =
            turnDirection * turnTorque * turnPowerByAngle * speedTurnMultiplier
            + dampingTorque;

        rb.AddTorque(Vector3.up * finalTorque, ForceMode.Force);

        Vector3 angularVelocity = rb.angularVelocity;
        angularVelocity.y = Mathf.Clamp(angularVelocity.y, -maxAngularSpeed, maxAngularSpeed);
        rb.angularVelocity = angularVelocity;
    }

    private bool HasEnoughWheelsOnGround()
    {
        int groundedCount = GetGroundedWheelCount();

        return groundedCount >= Mathf.CeilToInt(wheels.Count * 0.5f);
    }

    private int GetGroundedWheelCount()
    {
        int groundedCount = 0;

        foreach (WheelCollider wheel in wheels)
        {
            if (wheel.isGrounded)
                groundedCount++;
        }

        return groundedCount;
    }

    private void StopWheels()
    {
        foreach (WheelCollider wheel in wheels)
        {
            wheel.motorTorque = 0f;
            wheel.brakeTorque = brakePower;
        }

    }

    private void SpeedOnAnglesRipper(NavMeshAgent navMeshAgent)
    {
        Vector3 targetDirection = navMeshAgent.steeringTarget - transform.position;
        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude < 0.01f)
            return;

        float angle = Vector3.SignedAngle(transform.forward, targetDirection.normalized, Vector3.up);
        float absAngle = Mathf.Abs(angle);

        if (absAngle <= 90f)
        {
            directionOfAcceleraion = 1;

            float angle01 = Mathf.Clamp01(absAngle / 90f);
            currentPower = power * Mathf.Lerp(1f, 0.2f, angle01);
        }
        else
        {
            directionOfAcceleraion = -1;

            float backAngle = Mathf.Abs(180f - absAngle);
            float angle01 = Mathf.Clamp01(backAngle / 90f);

            currentPower = power * Mathf.Lerp(1f, 0.2f, angle01);
        }
    }

    private void LimitSpeed()
    {
        Vector3 velocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;

            rb.linearVelocity = new Vector3(
                limitedVelocity.x,
                velocity.y,
                limitedVelocity.z
            );
        }
    }

    public void HullStatsSetterer(string name, int mod)
    {
        HullMod hullStats = HSL.GetStats(name, "M" + mod);
        maxSpeed = hullStats.SPD;
        rotationSpeed = hullStats.ROTSPD;
        weight = hullStats.WEIGHT;
        power = hullStats.PWR;
        rb.mass = weight;
        Vector3 euler = rb.rotation.eulerAngles;
        rb.rotation = Quaternion.Euler(0f, euler.y, 0f);

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        HealthComponent HC = GetComponent<HealthComponent>();
        HC.Initialize(hullStats.HP);

    }

    private void ReferencesSetterer()
    {
        HSL = GetComponent<HullStatsLoader>();
        rb = GetComponent<Rigidbody>();
    }
}