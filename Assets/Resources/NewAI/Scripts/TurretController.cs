using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public class TurretController : MonoBehaviour
{

    private float minDamage;
    private float maxDamage;
    private float attackRange;

    public float turretRotationSpeed;
    public float turretEnergyCapacity;
    public float turretEnergyConsumption;
    public float turretReload;
    public float currentEnergy;
    public float whatTime;

    private float timeBetweenShots;
    private float timeStartShoot = 0f;
    private float timeBetweenReload = 0.5f;
    private float startReloadTime = 0f;

    private float targetYaw;
    private float turretYaw;
    private float inTurretYaw;
    private LayerMask enemyLayer;
    private float cooldownTime;
    private float startRotTime;
    private float timeBetweenRot = 5f;
    float timer;

    public List<Transform> muzzle = new List<Transform>();

    public Transform particleSystemTransform;

    private bool didSpot = false;
    private bool isShooting = false;

    private bool inBattle = false;
    private bool timeBeenSet = false;
    private bool rotating;
    private bool isRightMuzzle = true;
    private bool canRotate;
    private bool isBlue;
    public bool Twins;
    CanvasUI CUI;
    bool isInitialized = false;

    private void Awake()
    {
        ReferenceSetterer();
    }

    void Update()
    {
        if (!inBattle) TurretRotation();
        TurretCooldown();
        if (!isShooting)
        {
            if (Time.time - startReloadTime >= timeBetweenReload)
            {
                currentEnergy += (turretEnergyCapacity / turretReload) * Time.deltaTime;
                currentEnergy = Mathf.Clamp(currentEnergy, 0f, turretEnergyCapacity);
            }
        }
        if (isInitialized)
        {
            CUI = GetComponentInChildren<CanvasUI>();
            CUI.EnergyBar(currentEnergy);
        }
    }

    public void TurretRotation()
    {

        if (!canRotate)
        {
            TurretCooldown();
            return;
        }

        if (!rotating)
        {
            rotating = true;
            turretYaw = UnityEngine.Random.Range(-180f, 180f);
            startRotTime = Time.time;
        }

        Quaternion currentRotation = transform.localRotation;
        Quaternion desiredRotation = Quaternion.Euler(0f, turretYaw, 0f);
        transform.localRotation = Quaternion.RotateTowards(currentRotation, desiredRotation, turretRotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(currentRotation, desiredRotation) < 0.5f)
        {
            canRotate = false;
            rotating = false;
        }
    }

    public void TurretToPlayer(Vector3 currentTarget)
    {
        inBattle = true;

        Vector3 direction = currentTarget - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float turretYaw = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        Quaternion currentRotation = transform.localRotation;
        Quaternion desiredRotation = Quaternion.Euler(0f, transform.localRotation.eulerAngles.y + turretYaw, 0f);

        transform.localRotation = Quaternion.RotateTowards(currentRotation, desiredRotation, turretRotationSpeed * Time.deltaTime);

        if (IsInAttackRange(currentTarget) && Quaternion.Angle(transform.localRotation, desiredRotation) < 10f)
        {
            Attack(currentTarget);
        }
    }

    private void Attack(Vector3 currentTarget)
    {
        float energyCost = turretEnergyConsumption * whatTime;

        if (currentEnergy < energyCost)
        {
            isShooting = false;

            if (timeBeenSet == false)
            {
                startReloadTime = Time.time;
                timeBeenSet = true;
            }

            return;
        }

        if (Time.time - timeStartShoot < timeBetweenShots)
            return;

        AimToTheHull(currentTarget);

        int muzzleIndex = 0;

        if (Twins)
            muzzleIndex = isRightMuzzle ? 1 : 0;

        Transform activeMuzzle = muzzle[muzzleIndex];

        Vector3 direction = currentTarget - activeMuzzle.position;

        int enemyMask = isBlue
            ? LayerMask.GetMask("Red")
            : LayerMask.GetMask("Blue");

        int shootMask = enemyMask | LayerMask.GetMask("Map");

        /*   Debug.DrawRay(activeMuzzle.position, direction.normalized * attackRange, Color.red, 0.2f);

           if (Physics.Raycast(activeMuzzle.position, direction.normalized, out RaycastHit hit, attackRange, shootMask))
           {
               bool hitEnemy = (enemyMask & (1 << hit.collider.gameObject.layer)) != 0;

               if (hitEnemy)
               {*/
        if (Twins)
        {
            particleSystemTransform.position = activeMuzzle.position;
        }

        timeBeenSet = false;

        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();

        var collision = particleSystem.collision;
        collision.collidesWith = isBlue
            ? LayerMask.GetMask("Red", "Map")
            : LayerMask.GetMask("Blue", "Map");

        particleSystem.Emit(1);

        currentEnergy -= energyCost;
        isShooting = true;
        timeStartShoot = Time.time;

        isRightMuzzle = !isRightMuzzle;
    }
    /* else
     {
         // Першим raycast попав у Map, тобто стіна/перешкода блокує постріл
         isShooting = false;
  }  
 }*/


private void TurretCooldown()
    {
        if (Time.time - startRotTime >= timeBetweenRot)
        {
            canRotate = true;
        }
    }

    private void AimToTheHull(Vector3 currentTarget)
    {
        Vector3 targetPoint = currentTarget + Vector3.up * 0.7f;
        Vector3 direction = targetPoint - particleSystemTransform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        particleSystemTransform.forward = direction.normalized;
    }

    private bool IsInAttackRange(Vector3 currentTarget)
    {
        return Vector3.Distance(muzzle[0].position, currentTarget) <= attackRange;
    }

    private void ReferenceSetterer()
    {
        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>(true);
        var collision = particleSystem.collision;
        collision.enabled = true;
        collision.collidesWith = LayerMask.GetMask("Red", "Blue", "Map");
    }

    public void TurretStatsSetter(string turretName, int turretMod, bool blue, float turretRange, LayerMask layer)
    {
        if (turretName == "Firebird" || turretName == "Isida" || turretName == "Freeze")
        {
            whatTime = Time.deltaTime;
        }
        else
        {
            whatTime = 1f;
        }

        if (turretName == "Twins")
        {
            Twins = true;
            timeBetweenReload = 0f;
        }

        attackRange = turretRange;
        enemyLayer = layer;
        isBlue = blue;
        TurretControlBase turretControlBase = GetComponent<TurretControlBase>();
        TurretControlBase turretControl = turretControlBase.GetTurretControl(turretName, "M" + turretMod);
        turretRotationSpeed = turretControl.GetRotateSpeed() * 4;
        turretEnergyCapacity = turretControl.EnergyCapacity();
        currentEnergy = turretEnergyCapacity;
        turretEnergyConsumption = turretControl.EnergyConsumption();
        turretReload = turretControl.ReloadTime();
        if (turretName == "Shaft") turretReload /= 16;
        minDamage = turretControl.MinDamage();
        maxDamage = turretControl.MaxDamage();
        timeBetweenShots = turretControl.TimeBetweenShots();
        
        isInitialized = true;
    }
}
