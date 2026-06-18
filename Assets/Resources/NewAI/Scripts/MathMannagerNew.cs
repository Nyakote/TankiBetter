using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MatchManagerNew : MonoBehaviour
{
    public List<Transform> BlueBase;
    public List<Transform> RedBase;

    public GameObject enemyPrefab;
    public NavMeshAgent groundLayer;

    public int teamSize = 5;
    [Range(0, 3)] public int hullMod = 0;
    [Range(0, 3)] public int turretMod = 0;

    private List<GameObject> blueTeamTanks = new List<GameObject>();
    private List<GameObject> redTeamTanks = new List<GameObject>();

    private bool battleEnded = false;

    GameDataManager gameDataManager;
    public Text winScreen;
    public Text teamScreen;


    private void Start()
    {
        gameDataManager = GameDataManager.Instance;
        SpawnTeam(true);  // Blue team
        SpawnTeam(false); // Red team
    }

    void SpawnTeam(bool isBlue)
    {
        List<Transform> spawnGroup = isBlue ? BlueBase : RedBase;
        List<GameObject> teamList = isBlue ? blueTeamTanks : redTeamTanks;

        for (int i = 0; i < teamSize; i++)
        {
            Vector3 spawnPoint = GetRandomSpawnPoint(spawnGroup[UnityEngine.Random.Range(0, spawnGroup.Count)]);
            GameObject tank = Instantiate(enemyPrefab, spawnPoint, Quaternion.Euler(0, UnityEngine.Random.Range(-180, 180), 0));

            EnemyCreatorNew creator = tank.GetComponent<EnemyCreatorNew>();
            if (creator != null)
            {
                creator.isBlue = isBlue;
                creator.Initialize(groundLayer, hullMod, turretMod);
            }

            teamList.Add(tank);

            StartCoroutine(RegisterHealthAfterSpawn(tank, isBlue));
        }
    }

    private Vector3 GetRandomSpawnPoint(Transform baseZone)
    {
        float x = UnityEngine.Random.Range(-baseZone.localScale.x / 2f, baseZone.localScale.x / 2f);
        float z = UnityEngine.Random.Range(-baseZone.localScale.z / 2f, baseZone.localScale.z / 2f);

        return baseZone.position + new Vector3(x, 0f, z);
    }

    public void OnTankDeath(GameObject tank, bool isBlueTeam)
    {
        if (battleEnded) return;

        List<GameObject> teamList = isBlueTeam ? blueTeamTanks : redTeamTanks;
        teamList.Remove(tank);

        if (teamList.Count == 0)
        {
            battleEnded = true;
            EndBattle(isBlueTeam ? "Red" : "Blue", isBlueTeam); // Opposite team wins
        }
    }

    private IEnumerator RegisterHealthAfterSpawn(GameObject tank, bool isBlue)
    {
        yield return null; // чекаємо 1 кадр, щоб EnemyCreatorNew.Start() створив корпус

        HealthComponent health = tank.GetComponentInChildren<HealthComponent>();

        if (health == null)
        {
            Debug.LogError("HealthComponent not found in " + tank.name);
            yield break;
        }

        health.OnDeath += () => OnTankDeath(tank, isBlue);

        Debug.Log("Registered death event for " + tank.name);
    }

    private void EndBattle(string winningTeam, bool isBlueWinner)
    {
        if (gameDataManager.chosenTeam == winningTeam)
        {
            gameDataManager.playerBalance += gameDataManager.currentBetAmount * 2;
            teamScreen.text = winningTeam + " Win";
            teamScreen.color = isBlueWinner ? Color.red : Color.blue;
            winScreen.text = "You Won";
            winScreen.color = Color.green;
        }
        else
        {
            teamScreen.text = winningTeam + " Win";
            teamScreen.color = isBlueWinner ? Color.red : Color.blue;
            winScreen.text = "You Lose";
            winScreen.color = Color.red;
        }

        Invoke("BackToMenu", 2f);
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("BettingScreen");
    }
}
