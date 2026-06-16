using System.Collections.Generic;
using UnityEngine;

public class AiCalculator
{
    private readonly Dictionary<string, int> Score = new Dictionary<string, int>()
    {
        { "Firebird", -10 },
        { "Freeze", -10 },
        { "Twins", -5 },
        { "Ricochet", -5 },
        { "Thunder", 2 },
        { "Smoky", 2 },
        { "Railgun", 7 },
        { "Shaft", 7 }
    };

    public int Calc(string turretName)
    {
        if (!Score.TryGetValue(turretName, out int turretScore))
        {
            Debug.LogError($"Unknown turret name: {turretName}");
            return 0;
        }

        int hullBonus = Random.Range(1, 4); 

        int finalScore = turretScore + hullBonus;

        return Mathf.Clamp(finalScore, -10, 10);
    }
}