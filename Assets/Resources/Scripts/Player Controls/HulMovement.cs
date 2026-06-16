using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System;


public class HullMovement : MonoBehaviour
{
    [Header("Stats")]
    public float HP;
    public float speed;
    public float turnSpeed;
    public float weight;
    public float power;
    float stoppingFactor = 4f;
    float turningstopFactor = 4f;
    float accelFactor = 10;
    float turnAccelFactor = 8;

    [Header("Movement Settings")]
    float lastMovementInput;
    float lastTurnInput;
    float speedieneess;
    LayerMask cantGo;
    private bool isTouchingObstacle = false;
    private bool isGrounded;
    private bool isInitialized = false;

    [Header("References")]
    private Rigidbody rb;
    private HullStatsLoader hsl;
    private float moveInputZ = 0f;
    private float turnInput = 0f;
    HealthComponent health;
    GroundChecker gr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hsl = GetComponent<HullStatsLoader>();
        cantGo = LayerMask.GetMask("Props"); 
        Invoke("StatsSetter", 0.3f);
    }

    public void OnMovement(InputValue value)
    {
        Vector3 movement = value.Get<Vector3>();
        moveInputZ = movement.z;
    }
    public void OnTurning(InputValue value)
    {
        turnInput = value.Get<float>();
    }

    
    private void FixedUpdate()
    {
        if (!isInitialized) return;
        isGrounded = gr.FrontCheck || gr.MidCheck || gr.RearCheck;
        MovingCall();
    }

    void StatsSetter()
    {
        health = GetComponent<HealthComponent>();
        gr = GetComponent<GroundChecker>();
        string name = transform.name.Split(' ')[1];
        string hullMod = transform.name.Split(' ')[3];

        HullMod hullStats = hsl.GetStats(name, hullMod);
        int currentMod = Convert.ToInt32(hullMod.Substring(1));
        float speedBuff = 2 + 0.2f * currentMod;

        HP = hullStats.HP;
        speed = hullStats.SPD;
        speedieneess = speed;
        turnSpeed = hullStats.ROTSPD;
        weight = hullStats.WEIGHT;
        power = hullStats.PWR;

        Invoke(nameof(InitializeHealthWrapper), 0.5f);
        isInitialized = true;
    }
    public void HullMoving()
    {
        if (!(gr.FrontCheck || gr.RearCheck || gr.MidCheck)) return;
        if (isTouchingObstacle) return;
        if (accelFactor > 1) accelFactor -= 0.03f;
        Vector3 forwardMove = transform.forward * moveInputZ * (speed / accelFactor) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove);
        stoppingFactor = 1f;
        lastMovementInput = moveInputZ;
    }

    public void HullSlowing()
    {
        float limit;
        if (!isGrounded) limit = 6f;
        else limit = 5f;
        if (stoppingFactor != speed)
        {
            stoppingFactor += 0.02f;
            if (stoppingFactor > limit) stoppingFactor = speed;
            Vector3 forwardMove = lastMovementInput * transform.forward * speed / stoppingFactor * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardMove);
            accelFactor = 6;
        }
        
        
    }
    

    public void HullTurning()
    {
        if (!(gr.FrontCheck || gr.RearCheck || gr.MidCheck)) return;
        if (turnAccelFactor > 1) turnAccelFactor -= 0.1f;
        float turnAmount = turnInput * (turnSpeed / turnAccelFactor) * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        turningstopFactor = 1f;
        lastTurnInput = turnInput;
    }
    public void HullTurrningSlowing()
    {

        if (turningstopFactor != turnSpeed)
        {
            turningstopFactor += 0.2f;
            if (turningstopFactor > 3) turningstopFactor = turnSpeed;
            float turnAmount = lastTurnInput * turnSpeed / turningstopFactor * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }
        turnAccelFactor = 6;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & cantGo) != 0)
        {
            isTouchingObstacle = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & cantGo) != 0)
        {
            isTouchingObstacle = false;
        }
    }

    void InitializeHealthWrapper()
    {
        health.Initialize(HP);
    }
    private void MovingCall()
    {
        if (moveInputZ != 0 && isGrounded)
        {
            HullMoving();
        }
        else
        {
            HullSlowing();
        }

        if (turnInput != 0 && isGrounded)
        {
            HullTurning();
        }
        else
        {
            HullTurrningSlowing();
        }
    }

}
