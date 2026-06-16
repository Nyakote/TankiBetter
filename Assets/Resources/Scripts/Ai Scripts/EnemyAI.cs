using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using System.Collections;
using static UnityEngine.ParticleSystem;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    #region Variables
    #region Tank Variables

    [Header("Tank Stats")]
    public float hullMaxSpeed;
    public float hullPower;
    public float hullWeight;
    public float hullRotationSpeed;
    public float tankHealsPoints;

    public float currentHealth;
    public float turretRotationSpeed;
    public float turretEnergyCapacity;
    public float turretEnergyConsumption;
    public float turretReload;
    public float currentEnergy;
    public float whatTime;

    private int hullMod;
    private int turretMod;
    private string hullName;
    private string turretName;
    Transform muzzle;
    Transform muzzle2;
    Transform turret;
    Transform particleSystemTransform;


    float stoppingFactor = 1f;
    float turningstopFactor = 1f;
    float accelFactor = 3f;
    float turnAccelFactor = 3f;

    [Header("AI Controls")]

    float sightRange = 1000f;
    float attackRange;
    float closeRange = 100f;

    #endregion

    #region Hull Movement Variables

    private float walkToPointRange = 100f;
    private float angle;
    private Vector3 randomPos;
    private float maxTurn;

    #endregion

    #region Turret Movement Variables

    private float targetYaw;
    private float turretYaw;
    private float inTurretYaw;
    private float sightOfChecking = 90f;

    #endregion

    #region Debuggers
    public float stoppingDistance = 3f;
    private Transform currentTarget;

    private bool isInAttackRange;
    private bool isInSightRange;
    private bool isTooClose;
    private bool didSpot = false;
    private bool isShooting = false;
    private float timeBetweenShots;
    private float timeStartShoot = 0f;

    private float timeBetweenReload = 0.5f;
    private float startReloadTime = 0f;
    private bool timeBeenSet = false;

    private bool isRightMuzzle = true;

    private bool finishedMoving;
    private bool walkToPos;

    private bool rotating;
    private bool canRotate;

    private bool isInitialized = false;

    public string enemyTag;

    float distanceDeadZone = 20f;
    float repickMinDistance;
    float repickMaxDistance;
    float repickTime = 10f;
    float repickTimer = 10f;
    #endregion

    #region Cooldowns

    private float moveStartTime;
    private float maxTimeToReach = 10f;
    private float cooldownStartTime;
    int aiScore;
    private float cooldownTime;
    private float startRotTime;
    private float timeBetweenRot = 5f;
    float timer;
    #endregion

    private Vector3 lineOfSightMovePoint;
    private float nextLineOfSightMoveTime;
    private float lineOfSightMoveDelay = 0.8f;
    float preferredDistance;
    float distanceTolerance = 20f;
    #region References

    Rigidbody rb;
    HullStatsLoader HSL;
    NavMeshAgent NavMeshAgent;
    TurretControlBase turretControlBase;
    TurretControlBase turretControl;
    HealthComponent healthComponent;
    private AiCalculator aiCalculator;
    private AiRangeData aiRangeData;

    public string AiType;
    #endregion

    #region Shoot

    private ParticleSystem.Particle[] particles;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem.MainModule mainModule;

    #endregion

    private float minDamage;
    private float maxDamage;
    private bool isBlue;
    private LayerMask enemyLayer;
    string teamTag;
    #endregion

    #region Unity Methods


    private void Start()
    { 
        Invoke("ReferencesSetter",0.5f);
    }
    private void Update()
    {
        if (repickTimer == 0)
        {
            preferredDistance = UnityEngine.Random.Range(
         aiRangeData.MinDistance,
         aiRangeData.MaxDistance
         );

            repickMinDistance = Mathf.Max(aiRangeData.MinDistance, preferredDistance - distanceTolerance);
            repickMaxDistance = Mathf.Min(aiRangeData.MaxDistance, preferredDistance + distanceTolerance);
            AiType = (repickMinDistance + "" + repickMaxDistance);
        }
        else
        {
            repickTimer -= Time.deltaTime;

            if (repickTimer < 0f)
                repickTimer = 0f;
        }


        }
    private void LateUpdate()
    {
        if (!isInitialized) return;
        NavMeshAgent.nextPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;
        if(healthComponent.currentHealth < healthComponent.maxHealth) { didSpot = true; }
        SerchingForPlayer();

        if (!isShooting)
        {
            if (Time.time - startReloadTime >= timeBetweenReload)
            {
                currentEnergy += (turretEnergyCapacity / turretReload) * Time.deltaTime;
                currentEnergy = Mathf.Clamp(currentEnergy, 0f, turretEnergyCapacity);
            }
        }
        if (isInSightRange && isInAttackRange) {
            MoveCloser();

           // Debug.Log("Attack"); 
        }

        else if (isInSightRange && !isInAttackRange || didSpot)         //If there is player = Move closer
        {
            didSpot = true;
            MoveCloser();
            isShooting = false;
            if (timeBeenSet == false)
            {
                startReloadTime = Time.time;
                timeBeenSet = true;
            } // Debug.Log("Moving Closer");
        }

        else
        {
            Patrol();
            isShooting = false;
            if (timeBeenSet == false)
            {
                startReloadTime = Time.time;
                timeBeenSet = true;
            }            // Debug.Log("Patrolling");
        }
        
    }

    #endregion

    #region Patrol Movement

    public void Patrol()
    {
        TurretRotation();

        if (!finishedMoving)
        {
            if (!walkToPos)
            {
                SearchForPoint();
            }

            if (walkToPos)
            {
                if (Time.time - moveStartTime > maxTimeToReach)
                {
                    walkToPos = false;
                    finishedMoving = true;
                    cooldownStartTime = Time.time;
                    MovingCooldown();
                    return;
                }

                maxTurn = hullRotationSpeed * Time.fixedDeltaTime;
                accelFactor = Mathf.MoveTowards(accelFactor, 1f, Time.fixedDeltaTime);
                MoveTowardDirection(randomPos);

                if (Vector3.Distance(transform.position, randomPos) < 1f)
                {
                    walkToPos = false;
                    finishedMoving = true;
                    cooldownStartTime = Time.time;
                    MovingCooldown();
                }
            }
        }
        else { MovingCooldown(); }
    }

    void SearchForPoint()
    {
        float randomX = UnityEngine.Random.Range(-walkToPointRange, walkToPointRange);
        float randomZ = UnityEngine.Random.Range(-walkToPointRange, walkToPointRange);
        randomPos = new Vector3(randomX, 0, randomZ) + transform.position;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            randomPos = hit.position;
            if (float.IsNaN(randomPos.x) || float.IsNaN(randomPos.y) || float.IsNaN(randomPos.z))
            {
                Debug.LogError("Sampled NavMesh position is NaN!");
                walkToPos = false;
                return;
            }
            NavMeshAgent.SetDestination(randomPos);
            moveStartTime = Time.time;
            maxTimeToReach = Vector3.Distance(randomPos, transform.position) / hullMaxSpeed;


            walkToPos = true;
        }
        else
        {
            walkToPos = false;
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

        Quaternion currentRotation = turret.transform.localRotation;
        Quaternion desiredRotation = Quaternion.Euler(0f, turretYaw, 0f);
        turret.transform.localRotation = Quaternion.RotateTowards(currentRotation, desiredRotation, turretRotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(currentRotation, desiredRotation) < 0.5f)
        {
            canRotate = false;
            rotating = false;
        }
    }
    #endregion

    #region Move Closer 

    public void MoveCloser()
    {
        TurretToPlayer();

        if (currentTarget == null)
        {
            didSpot = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (!HasLineOfSight())
        {
            isShooting = false;
            MoveToFindLineOfSight();
            return;
        }
        // Занадто близько — від'їжджаємо
        if (distance < repickMinDistance)
        {
         
            MoveAwayFromTarget();
            return;
        }

        // Занадто далеко — їдемо ближче
        if (distance > repickMaxDistance)
        {
            NavMeshAgent.SetDestination(currentTarget.position);
            MoveTowardDirection(currentTarget.position);
            return;
        }

        // Нормальна дистанція — стоїмо
        StopMoving();
    }
    private void StopMoving()
    {

        NavMeshAgent.ResetPath();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        NavMeshAgent.nextPosition = transform.position;
    }
    private void MoveAwayFromTarget()
    {
        if (currentTarget == null)
            return;

   
        Vector3 awayDirection = (transform.position - currentTarget.position).normalized;
        Vector3 desiredPosition = transform.position + awayDirection * 20f;

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 30f, NavMesh.AllAreas))
        {
            NavMeshAgent.SetDestination(hit.position);
            MoveTowardDirection(hit.position);
        }
    }
    private void TurretToPlayer()
    {
        if (currentTarget == null || turret == null)
            return;

        Vector3 direction = currentTarget.position - turret.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float turretYaw = Vector3.SignedAngle(turret.forward, direction, Vector3.up);

        Quaternion currentRotation = turret.localRotation;
        Quaternion desiredRotation = Quaternion.Euler(0f, turret.localRotation.eulerAngles.y + turretYaw, 0f);

        turret.localRotation = Quaternion.RotateTowards(currentRotation, desiredRotation, turretRotationSpeed * Time.deltaTime);

        if (isInAttackRange && Quaternion.Angle(turret.localRotation, desiredRotation) < 10f)
        {
            Attack();
        }
    }


    private void Attack()
    {
        if (currentEnergy >= turretEnergyConsumption * whatTime)
        {
            if (Time.time - timeStartShoot >= timeBetweenShots)
            {
                if (turretName == "Twins") { particleSystemTransform.transform.position = isRightMuzzle ? muzzle.position : muzzle2.position; }
                timeBeenSet = false;
                ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
                var collision = particleSystem.collision;
                collision.collidesWith = isBlue ? LayerMask.GetMask("Red", "Map") : LayerMask.GetMask("Blue", "Map");

                particleSystem.Emit(1);
                currentEnergy -= turretEnergyConsumption * whatTime;
                isShooting = true;
                timeStartShoot = Time.time;
                isRightMuzzle = !isRightMuzzle;
            }
        }
        else
        {

            isShooting = false;
            if (timeBeenSet == false)
            {
                startReloadTime = Time.time;
                timeBeenSet = true;
            }
        }

    }

    private void MoveToFindLineOfSight()
    {
        if (currentTarget == null)
            return;

        if (Time.time >= nextLineOfSightMoveTime || Vector3.Distance(transform.position, lineOfSightMovePoint) < 2f)
        {
            Vector3 toTarget = currentTarget.position - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.01f)
                return;

            toTarget.Normalize();

            Vector3 sideDirection = Vector3.Cross(Vector3.up, toTarget);

            if (UnityEngine.Random.value > 0.5f)
                sideDirection = -sideDirection;

            Vector3 desiredPosition = transform.position + sideDirection * UnityEngine.Random.Range(10f, 25f);

            if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                lineOfSightMovePoint = hit.position;
                nextLineOfSightMoveTime = Time.time + lineOfSightMoveDelay;
            }
        }

        NavMeshAgent.SetDestination(lineOfSightMovePoint);
        MoveTowardDirection(lineOfSightMovePoint);
    }
    #endregion
    private bool HasLineOfSight()
    {
        if (currentTarget == null || muzzle == null)
            return false;

        Vector3 origin = muzzle.position;
        Vector3 targetPoint = currentTarget.position + Vector3.up * 0.8f;

        Vector3 direction = targetPoint - origin;
        float distance = direction.magnitude;

        if (distance < 0.01f)
            return true;

        int wallMask = LayerMask.GetMask("Map", "Props");

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, wallMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
            return false;
        }

        Debug.DrawLine(origin, targetPoint, Color.green);
        return true;
    }
    #region Cooldowns

    private void MovingCooldown()
    {
        if (Time.time - cooldownStartTime >= cooldownTime)
        {
            finishedMoving = false;
            walkToPos = false;
            cooldownTime = UnityEngine.Random.Range(1, 15);
        }
    }

    private void TurretCooldown()
    {
        if (Time.time - startRotTime >= timeBetweenRot)
        {
            canRotate = true;
        }
    }

    #endregion

    #region Called in Methods

    public void SerchingForPlayer()
    {
        currentTarget = null;
        float closestDist = Mathf.Infinity;

        // Search in sight range
        Collider[] hits = Physics.OverlapSphere(muzzle.position, sightRange, enemyLayer);
        foreach (var hit in hits)
        {
            Vector3 direction = hit.transform.position - muzzle.position;
            float angle = Vector3.SignedAngle(muzzle.forward, direction, Vector3.up);

            if (Mathf.Abs(angle) < sightOfChecking) // In front of turret
            {
                float dist = Vector3.Distance(muzzle.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    currentTarget = hit.transform;
                }
            }
        }

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(muzzle.position, currentTarget.position);
            isInSightRange = true;
            isInAttackRange = distanceToTarget <= attackRange;
            return;
        }

        // Too close check (from behind or side)
        Collider[] closeHits = Physics.OverlapSphere(muzzle.position, closeRange, enemyLayer);
        if (closeHits.Length > 0)
        {
            currentTarget = closeHits[0].transform;
            didSpot = true;
            isInSightRange = true;
            isInAttackRange = true;
        }
        else
        {
            didSpot = false;
            isInAttackRange = false;
            isInSightRange = false;
        }
    }

    public void MoveTowardDirection(Vector3 position)
    {
        Vector3 flatPosition = new Vector3(position.x, transform.position.y, position.z);
        float distanceToTarget = Vector3.Distance(transform.position, flatPosition);

        if (distanceToTarget <= stoppingDistance)
        {
            NavMeshAgent.ResetPath(); 

        
            Vector3 direction = (flatPosition - transform.position).normalized;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRot, maxTurn);
                rb.MoveRotation(newRot);
            }
            return;
        }

 
        Vector3 directionToMove;

        if (NavMeshAgent.hasPath)
            directionToMove = NavMeshAgent.steeringTarget - transform.position;
        else
            directionToMove = position - transform.position;

        directionToMove.y = 0;
        directionToMove.Normalize();

        bool backward = Vector3.SignedAngle(transform.forward, directionToMove, Vector3.up) > 90;
        Vector3 moveDirection = backward ? -transform.forward : transform.forward;
        float angle = Vector3.Angle(moveDirection, directionToMove);

        Vector3 lookDirection = backward ? -directionToMove : directionToMove;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, maxTurn);
        rb.MoveRotation(newRotation);

        float absAngle = Mathf.Abs(angle);
        float movementSpeed =
            absAngle < 30f ? hullMaxSpeed :
            absAngle < 60f ? hullMaxSpeed / 2f :
            absAngle < 100f ? hullMaxSpeed / 4f : 0f;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        if (!Physics.Raycast(rayOrigin, moveDirection, 1.5f, LayerMask.GetMask("Props")))
        {
            Vector3 move = moveDirection * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }

    #endregion

    #region Setterers

    public void Initialize(int hullModI, int turretModI, string hullNameI, string turretNameI, float radius, bool blue, string team)
    {
        rb = GetComponent<Rigidbody>();
        HSL = GetComponent<HullStatsLoader>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        healthComponent = GetComponent<HealthComponent>();
        aiCalculator = new AiCalculator();
        if (turretNameI == "Firebird" || turretNameI == "Freeze") attackRange = 30f;
        else if (turretNameI == "Twins" || turretNameI == "Ricochet") attackRange = 60f;
        else attackRange = 150f;
        hullMod = hullModI;
        turretMod = turretModI;
        hullName = hullNameI;
        turretName = turretNameI;
        isBlue = blue;
        teamTag = team;

        if (isBlue)
        {
            transform.tag = "Blue";
            gameObject.layer = 7;
            enemyTag = "Red";
            enemyLayer = LayerMask.GetMask("Red");
        }
        else
        {
            transform.tag = "Red";
            gameObject.layer = 6;
            enemyTag = "Blue";
            enemyLayer = LayerMask.GetMask("Blue");
        }

        NavMeshAgent.radius = radius;
        NavMeshAgent.updateRotation = true;
        NavMeshAgent.updatePosition = false;

        Invoke("HullStatsSetter", 0.5f);
        Invoke("TurretStatsSetter", 0.5f);
    }


    private void ReferencesSetter()
    {

        transform.tag = teamTag;

        ParticleSystem particleSystem = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
        var collision = particleSystem.collision;
        collision.enabled = true;
        collision.collidesWith = LayerMask.GetMask("Red", "Blue", "Map");
    }
    private void HullStatsSetter()
    {
        HullMod hullStats = HSL.GetStats(hullName, "M" + hullMod);
       
        //float speedBuff = 2f + 0.2f * hullMod;

        tankHealsPoints = hullStats.HP;
        hullMaxSpeed = hullStats.SPD;
        hullRotationSpeed = hullStats.ROTSPD;
        hullWeight = hullStats.WEIGHT;
        hullPower = hullStats.PWR;
        healthComponent.Initialize(tankHealsPoints);
    }

    private void TurretStatsSetter()
    {
        muzzle = transform.Find($"{hullName}(Clone)/mount/{turretName}(Clone)/muzzle");
        turret = transform.Find($"{hullName}(Clone)/mount/{turretName}(Clone)/");
        particleSystemTransform = transform.Find($"{hullName}(Clone)/mount/{turretName}(Clone)/Particle System");

        if (turretName == "Firebird" || turretName == "Isida" || turretName == "Freeze") whatTime = 1 * Time.deltaTime;
        else whatTime = 1;
        if (turretName == "Twins")
        {
            muzzle2 = transform.Find($"{hullName}(Clone)/mount/{turretName}(Clone)/muzzle2");
            timeBetweenReload = 0f;
        }
        turretControlBase = turret.GetComponent<TurretControlBase>();
        turretControl = turretControlBase.GetTurretControl(turretName, "M" + turretMod);    
        turretRotationSpeed = turretControl.GetRotateSpeed()*5;
        turretEnergyCapacity = turretControl.EnergyCapacity();
        currentEnergy = turretEnergyCapacity;
        turretEnergyConsumption = turretControl.EnergyConsumption();
        turretReload = turretControl.ReloadTime();
        minDamage = turretControl.MinDamage();
        maxDamage = turretControl.MaxDamage();
        timeBetweenShots = turretControl.TimeBetweenShots();

       
        Invoke("DelaySetter", 5f);
        
        
    }

    private void DelaySetter()
    {
        aiScore = aiCalculator.Calc(turretName);
        aiRangeData = AiTypePicker.PickAiType(aiScore, attackRange);
        AiType = (aiRangeData.MinDistance + " " + aiRangeData.MaxDistance);
        repickMinDistance = aiRangeData.MinDistance;
        repickMaxDistance = aiRangeData.MaxDistance;
        isInitialized = true;
    }
    #endregion

    #region Shoot

    public void HandleParticleCollision(GameObject other)
    {
                
        if (!other.CompareTag(enemyTag)) return;

        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        HealthComponent health = other.GetComponent<HealthComponent>();
        health.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(minDamage, maxDamage)));

        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 collisionPoint = collisionEvents[i].intersection;

            int closestIndex = -1;
            float closestDist = float.MaxValue;

            for (int j = 0; j < numParticlesAlive; j++)
            {
                float dist = Vector3.Distance(particles[j].position, collisionPoint);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = j;
                }
            }

            if (closestIndex >= 0 && closestDist < 0.5f)
            {
                particles[closestIndex].remainingLifetime = -1f;
            }
        }

        particleSystem.SetParticles(particles, numParticlesAlive);



    }

    #endregion
}