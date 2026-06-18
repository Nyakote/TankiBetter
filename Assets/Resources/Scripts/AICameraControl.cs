using UnityEngine;
using System.Collections.Generic;

public class AICameraControl : MonoBehaviour
{
    [Header("Camera settings")]
    float rise = -12f;
    
     float switchTime = 3f;
    public float followSmoothSpeed = 5f; 
    public float rotationSmoothSpeed = 5f;
    [SerializeField]
    private float radius = 7f;
    private float height = 4f;


    [Header("Teams to follow")]
    public Transform blueTeamParent;  // The GameObject parent of blue tanks
    public Transform redTeamParent;   // The GameObject parent of red tanks
    List<GameObject> blueTeamTanks => GetChildrenAsGameObjects(blueTeamParent);
    List<GameObject> redTeamTanks => GetChildrenAsGameObjects(redTeamParent);

    private GameObject currentTank;
    private GameObject hull;
    private GameObject turret;

    private float lastSwitchTime = 0f;

    void Start()
    {
        PickRandomTank();
        lastSwitchTime = Time.time;

    }


    private List<GameObject> GetChildrenAsGameObjects(Transform parent)
    {
        List<GameObject> tanks = new List<GameObject>();
        if (parent == null) return tanks;

        for (int i = 0; i < parent.childCount; i++)
        {
            tanks.Add(parent.GetChild(i).gameObject);
        }
        return tanks;
    }

    void Update()
    {
        if (Time.time - lastSwitchTime > switchTime)
        {
            PickRandomTank();
            lastSwitchTime = Time.time;
        }

        if (currentTank == null) return;

        if (hull == null || turret == null)
        {
            FindTankParts();
            if (hull == null) hull = currentTank;
            if (turret == null) turret = hull;
        }

        FollowTank();
    }
    private void FindTankParts()
    {
        hull = null;
        turret = null;

        Transform[] allChildren = currentTank.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            Transform mount = child.Find("mount");

            // Якщо в об'єкта є mount — це корпус
            if (mount != null)
            {
                hull = child.gameObject;

                // Башта — перший об'єкт всередині mount
                if (mount.childCount > 0)
                {
                    turret = mount.GetChild(0).gameObject;
                }

                return;
            }
        }

        // fallback, якщо нічого не знайшло
        hull = currentTank;
        turret = currentTank;

        Debug.LogWarning("Hull or turret not found for camera target: " + currentTank.name);
    }
    void PickRandomTank()
    {
        List<GameObject> combined = new List<GameObject>();

        combined.AddRange(blueTeamTanks);
        combined.AddRange(redTeamTanks);

        if (combined.Count == 0)
        {
            currentTank = null;
            return;
        }

        currentTank = combined[Random.Range(0, combined.Count)];
        hull = null;
        turret = null;
    }


    void FollowTank()
    {
        if (turret == null) return;

         // висота камери над баштою
        float distance = radius;    // відстань позаду башти

        // Камера стоїть позаду башти
        Vector3 desiredPosition =
            turret.transform.position
            - turret.transform.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmoothSpeed * Time.deltaTime
        );

        // Камера дивиться туди, куди повернута башта
        Quaternion desiredRotation = Quaternion.LookRotation(
            turret.transform.forward,
            Vector3.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }

    string GetTurretName()
    {
        return "mount";
    }
}
