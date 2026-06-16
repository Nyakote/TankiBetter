using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class PositionChoser : MonoBehaviour
{
    [Header("Temp")]
    float timer;
    [SerializeField] private float sideWidth = 200f;
    float thisTimeWidth = 0f;
    [SerializeField] private float navMeshSearchRadius = 100f;

    float checktime = 5f;
    float checktimer;
    private float anotherchecktime = 0.5f;
    float anotherchecktimer, radius, sideOffset;

    public float initializationTime = 3f;
    public float detectionRadius = 800f;

    private Transform currentTarget;
    private float minDistance;
    private float maxDistance;

    private LayerMask enemyLayer;

    NavMeshAgent navMeshAgent;
    HullController hullController;
    TurretController turretController;

    
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
            if (checktimer == 0)
            {
                thisTimeWidth = UnityEngine.Random.Range(0, sideWidth);
                radius = UnityEngine.Random.Range(minDistance, maxDistance);
                Debug.Log(thisTimeWidth + " " + radius);
                checktimer = checktime;
            }
            else
            {
                checktimer -= Time.deltaTime;
                if (checktimer <= 0) checktimer = 0;
            }

            if (anotherchecktimer == 0)
            {
                if (currentTarget != null)
                    GetDesiredPosition();
                else
                    GetRandomPose();
                anotherchecktimer = anotherchecktime;
            }
            else
            {
                anotherchecktimer -= Time.deltaTime;
                if (anotherchecktimer <= 0) anotherchecktimer = 0;
            }
            DetectEnemy();
            hullController = GetComponentInChildren<HullController>();
            hullController.HullMovement(navMeshAgent);
            turretController = GetComponentInChildren<TurretController>();
            turretController.TurretToPlayer(currentTarget.position);
            navMeshAgent.nextPosition = hullController.transform.position;
            navMeshAgent.radius = hullController.transform.localScale.z+1.5f;

        }

     /*   if (currentTarget != null)
        {
            Debug.DrawLine(transform.position, currentTarget.position, Color.green);
        }*/
        
    }

    private void GetDesiredPosition()
    {
        if (currentTarget == null)
            return;

       
        sideOffset = UnityEngine.Random.Range(-thisTimeWidth / 2f, thisTimeWidth / 2f);

        Vector3 directionFromTarget = transform.position - currentTarget.position;
        directionFromTarget.y = 0f;

        if (directionFromTarget.sqrMagnitude < 0.01f)
            directionFromTarget = -currentTarget.forward;

        directionFromTarget.Normalize();

        Vector3 sideDirection = Vector3.Cross(Vector3.up, directionFromTarget).normalized;

        Vector3 desiredPosition = currentTarget.position + directionFromTarget * radius + sideDirection * sideOffset;

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
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
    }


}
