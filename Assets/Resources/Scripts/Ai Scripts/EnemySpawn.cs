using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] int enemyCount = 10;
    [SerializeField] GameObject enemyRoot;
    [SerializeField] string minMod;
    [SerializeField] string maxMod;

    List<Vector3> takenPos = new List<Vector3>();
    EnemyCreator ec;

    void Start()
    {
        ec = GetComponent<EnemyCreator>();
        takenPos.Add(new Vector3(0, 0.5f, 0)); // optional starting block
        EnemySummoner();
    }

    void EnemySummoner()
    {
        Debug.Log("Start spawning");
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyPlacer();
        }
    }

    void EnemyPlacer()
    {
        int modHull = UnityEngine.Random.Range(Convert.ToInt32(minMod.Substring(1)), Convert.ToInt32(maxMod.Substring(1)) + 1);
        int modTurret = UnityEngine.Random.Range(Convert.ToInt32(minMod.Substring(1)), Convert.ToInt32(maxMod.Substring(1)) + 1);

        Vector3 position;
        NavMeshHit hit;
        int attempt = 0;
        do
        {
            if (attempt++ > 100)
            {
                Debug.LogWarning("Could not find a valid spawn position after 100 attempts.");
                return;
            }

            position = EnemyPosCacl(NavMesh.CalculateTriangulation());

        } while (
            takenPos.Any(pos => Vector3.Distance(pos, position) < 2.0f) || // prevent overlapping
            !NavMesh.SamplePosition(position, out hit, 2f, NavMesh.AllAreas)
        );

        takenPos.Add(hit.position);
        Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        GameObject enemy = Instantiate(enemyRoot, hit.position, rotation);

        EnemyCreator enemyCreator = enemy.GetComponent<EnemyCreator>();
        enemyCreator.Initialize(enemy.GetComponent<NavMeshAgent>(), modHull, modTurret);
    }

    Vector3 EnemyPosCacl(NavMeshTriangulation navMesh)
    {
        int triangleIndex = UnityEngine.Random.Range(0, navMesh.indices.Length / 3) * 3;

        Vector3 v1 = navMesh.vertices[navMesh.indices[triangleIndex]];
        Vector3 v2 = navMesh.vertices[navMesh.indices[triangleIndex + 1]];
        Vector3 v3 = navMesh.vertices[navMesh.indices[triangleIndex + 2]];

        float a = UnityEngine.Random.value;
        float b = UnityEngine.Random.value;
        if (a + b > 1)
        {
            a = 1 - a;
            b = 1 - b;
        }
        float c = 1 - a - b;

        Vector3 point = a * v1 + b * v2 + c * v3;
        return point;
    }
}
