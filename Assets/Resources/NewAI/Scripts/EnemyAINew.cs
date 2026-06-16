using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyAINew : MonoBehaviour
{
    Dictionary<string, int> turretRanges = new Dictionary<string, int>(){
    { "Firebird", 20 },
    { "Freeze", 20 },
    { "Twins", 60 },
    { "Ricochet", 60 },
    { "Smoky", 80 },
    { "Thunder", 80 },
    { "Railgun", 150 },
    { "Shaft", 150 }
};

    TurretControlBase turretControlBase;
    TurretControlBase turretControl;

    private float startTime;
    private float startTimer;

    public string AITeam;
    private string enemyTag;
    private LayerMask enemyLayer;

    private int savedHullMod;
    private int savedTurretMod;
    private string savedHullName;
    private string savedTurretName;
    private bool savedBlue;
    private string savedTeam;

    public void OverallInitialization(int hullModI, int turretModI, string hullNameI, string turretNameI, bool blue, string team)
    {

        transform.name = (hullNameI + " " + turretNameI + " M" + hullModI + " M" + turretModI);
        AITeam = team;

        if (blue)
        {
            transform.tag = "Blue";
            gameObject.layer = 7;

            transform.GetChild(0).tag = "Blue";
            transform.GetChild(0).gameObject.layer = 7;
            enemyTag = "Red";
            enemyLayer = LayerMask.GetMask("Red");
        }
        else
        {
            transform.tag = "Red";
            gameObject.layer = 6;

            transform.GetChild(0).tag = "Red";
            transform.GetChild(0).gameObject.layer = 6;
            enemyTag = "Blue";
            enemyLayer = LayerMask.GetMask("Blue");
        }

        savedHullMod = hullModI;
        savedTurretMod = turretModI;
        savedHullName = hullNameI;
        savedTurretName = turretNameI;
        savedBlue = blue;
        savedTeam = team;

        Invoke("ReSender", 3f);
        Invoke("ReSender2", 1f);

    }

    private void ReSender()
    {
        HullController hullcontroller = GetComponentInChildren<HullController>();
        hullcontroller.HullStatsSetterer(savedHullName, savedHullMod);

        TurretController turretController = GetComponentInChildren<TurretController>();
        turretController.TurretStatsSetter(savedTurretName, savedTurretMod, savedBlue, turretRanges[savedTurretName],enemyLayer);


    }
    private void ReSender2()
    {
        AiCalculator aiCalculator = new AiCalculator();
        AiRangeData aiRangeData = new AiRangeData();
        aiRangeData = AiTypePicker.PickAiType(aiCalculator.Calc(savedTurretName), turretRanges[savedTurretName]);
        PositionChoser positionChoser = GetComponentInChildren<PositionChoser>();
        positionChoser.Initialize(enemyTag, enemyLayer, aiRangeData);
    }
}
