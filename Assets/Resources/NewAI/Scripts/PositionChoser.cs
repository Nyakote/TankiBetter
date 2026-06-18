using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class PositionChoser : MonoBehaviour
{
    [Header("Temp")]
    float timer;
    [SerializeField] private float sideWidth = 2000f;
    float thisTimeWidth = 0f;
    [SerializeField] private float navMeshSearchRadius = 10f;


    Vector3 anchor;
    float checktime = 0.75f;
    float checktimer;

    public float radius, sideOffset;
    public float initializationTime = 3f;
    public float detectionRadius = 800f;

    private Transform currentTarget;
    private float minDistance;
    private float maxDistance;

    private LayerMask enemyLayer;

    private bool isAnchorSet;
    public bool isReachedPreviousPos = true;
    float angleOfTerritory = 30f;
    float timeToReach = 10f;
    float timerToReach;
    float randomAngle;
    float randomRadius;
    LayerMask mapMask;

    NavMeshAgent navMeshAgent;
    HullController hullController;
    TurretController turretController;
    float counter = 0;
    float keepclosedistance = 10f;
    float keepTimer;
    
    void Awake()
    {
        timer = Time.time;
        navMeshAgent  = GetComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        
    }

    void Update()
    {
        if (Time.time - timer >= initializationTime)
        {
            hullController = GetComponentInChildren<HullController>();
            turretController = GetComponentInChildren<TurretController>();
            DetectEnemy();
            if (currentTarget != null)
            {
                if (Vector3.Distance(currentTarget.position, anchor) < minDistance || Vector3.Distance(currentTarget.position, anchor) > maxDistance) {isAnchorSet = false; isReachedPreviousPos=true;}
                if (Vector3.Distance(hullController.transform.position, navMeshAgent.destination) < 5f) isReachedPreviousPos = true;
                if (timerToReach >= timeToReach) { isReachedPreviousPos = true; timerToReach = 0; }
            }
            if (isAnchorSet)
            {
                if (checktimer != 0) { checktimer -= Time.deltaTime; if (checktimer < 0) checktimer = 0; }
                else
                {
                    randomAngle = UnityEngine.Random.Range(-angleOfTerritory, angleOfTerritory);
                    randomRadius = UnityEngine.Random.Range(0f, maxDistance/2);
                    checktimer = checktime;
                }
                KeepPositionUntil();
            }
            else
            {
                if (currentTarget != null)
                {
                    if (counter >= 3) { radius = minDistance; keepTimer = keepclosedistance; counter = 0; }
                    
                    if(keepTimer != 0)
                    {
                        keepTimer -= Time.deltaTime;
                        if (keepTimer < 0) keepTimer = 0;
                    }
                    else
                    {
                        thisTimeWidth = UnityEngine.Random.Range(0, sideWidth);
                        radius = UnityEngine.Random.Range(minDistance, maxDistance);
                    }
                    GetDesiredPosition();
                }
                else
                    GetRandomPose();
            }

            hullController.SetAgent(navMeshAgent);
            if (currentTarget != null && turretController != null)
            {
                turretController.TurretToPlayer(currentTarget.position);
            }
            navMeshAgent.nextPosition = hullController.transform.position;
            navMeshAgent.radius = hullController.transform.localScale.z+1.5f;
            timerToReach += Time.deltaTime;
        }

        
    }

    private void KeepPositionUntil()
    {
        if (!isReachedPreviousPos) return;
        if (currentTarget == null) return;

        for (int i = 0; i < 15; i++)
        {
            randomAngle = UnityEngine.Random.Range(-angleOfTerritory, angleOfTerritory);
            randomRadius = UnityEngine.Random.Range(0f, maxDistance);

            Vector3 forward = anchor - currentTarget.position;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.01f)
                forward = transform.forward;

            forward.Normalize();

            Vector3 keepDirection = Quaternion.Euler(0f, randomAngle, 0f) * forward;
            Vector3 newTerritoryPos = anchor + keepDirection * randomRadius;

            if (!NavMesh.SamplePosition(newTerritoryPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                continue;

            Vector3 from = hit.position + Vector3.up * 2f;
            Vector3 to = currentTarget.position + Vector3.up * 2f;
            
            if (Vector3.Distance(hullController.transform.position, currentTarget.position) < maxDistance)
            {
                if (Physics.Linecast(from, to, mapMask))
                {
                    counter+=Time.deltaTime;
                    continue;
                }
            }
            navMeshAgent.SetDestination(hit.position);

            isReachedPreviousPos = false;
            timerToReach = 0f;

            return;
        }

        isReachedPreviousPos = true;
    }

    private void GetDesiredPosition()
    {
        if (currentTarget == null)
            return;

        sideOffset = UnityEngine.Random.Range(-thisTimeWidth, thisTimeWidth);

        Vector3 directionFromTarget = transform.position - currentTarget.position;
        directionFromTarget.y = 0f;

        if (directionFromTarget.sqrMagnitude < 0.01f)
            directionFromTarget = -currentTarget.forward;

        directionFromTarget.Normalize();

        Vector3 sideDirection = Vector3.Cross(Vector3.up, directionFromTarget).normalized;

        Vector3 desiredPosition = currentTarget.position + directionFromTarget * radius + sideDirection * sideOffset;

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            anchor = hit.position;
            isAnchorSet = true;
        }
    }

    public void DetectEnemy()
    {
        currentTarget = null;
        float closestDist = Mathf.Infinity;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);


        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (closestDist > dist)
            {
                closestDist = dist;
                currentTarget = hit.transform;
            }
        }
    }

    private void GetRandomPose() 
    {
        float randomX = UnityEngine.Random.Range(-50, 50);
        float randomZ = UnityEngine.Random.Range(-50, 50);
        Vector3 randomPos = new Vector3(randomX, 0, randomZ) + transform.position;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            randomPos = hit.position;
            if (float.IsNaN(randomPos.x) || float.IsNaN(randomPos.y) || float.IsNaN(randomPos.z))
            {
                return;
            }
            navMeshAgent.SetDestination(randomPos); 
        }
    }

  

    public void Initialize(string tag, LayerMask layer, AiRangeData data)
    {
        enemyLayer = layer;
        minDistance = data.MinDistance;
        maxDistance = data.MaxDistance;
        mapMask = LayerMask.GetMask("Map");
    }


}
