using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AI;

using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class MatchManager : MonoBehaviour

{

    public GameObject enemyPrefab;

    public Transform blueSpawnGroup;

    public Transform redSpawnGroup;

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

        Transform spawnGroup = isBlue ? blueSpawnGroup : redSpawnGroup;

        List<GameObject> teamList = isBlue ? blueTeamTanks : redTeamTanks;



        for (int i = 0; i < teamSize; i++)

        {

            Transform spawnPoint = spawnGroup.GetChild(i);

            GameObject tank = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);



            EnemyCreator creator = tank.GetComponent<EnemyCreator>();

            if (creator != null)

            {

                creator.isBlue = isBlue; // Assign team color

                creator.Initialize(groundLayer, hullMod, turretMod);

            }



            HealthComponent health = tank.GetComponent<HealthComponent>();

            if (health != null)

            {

                health.OnDeath += () => OnTankDeath(tank, isBlue);

            }



            teamList.Add(tank);

        }

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
