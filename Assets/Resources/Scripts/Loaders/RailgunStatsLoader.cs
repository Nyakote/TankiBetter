using System;
using System.Collections.Generic;
using UnityEngine;

public class RailgunStatsLoader : MonoBehaviour
{
    public TurretDatabase TurretStats;

    private void Awake()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/RailgunSTATS");

        if (jsonText != null)
        {
            TurretStats = JsonUtility.FromJson<TurretDatabase>(jsonText.text);
        }
        else
        {
            Debug.LogError("Couldn't find RailgunSTATS.json in Resources!");
        }
    }
    public (TurretMod mod, TurretFixed fixeds) GetFullStats(string turretName, string modLevel)
    {
        TurretDefinition turret = TurretStats.turret.Find(t => t.name == turretName);
        if (turret == null) return (null, null);

        TurretMod mod = turret.mods.Find(m => m.mod == modLevel);
        TurretFixed fixeds = turret.fixeds != null && turret.fixeds.Count > 0 ? turret.fixeds[0] : null;

        return (mod, fixeds);
    }

    [Serializable]
    public class TurretMod
    {
        public string mod;
        public int price;
        public int min_damage;
        public int max_damage;
        public float impact_force;
        public float recoil;
        public float rotation_speed;
        public float rotation_acceleration;
        public float reload_time;
        public float charge_time;
        public float delay;
        public float piercing_damage_percent;
    }


    [Serializable]
    public class TurretFixed
    {
        public int auto_aim_up;
        public int auto_aim_down;
    }

    [Serializable]
    public class TurretDefinition
    {
        public string name;
        public string type;
        public List<TurretMod> mods;
        public List<TurretFixed> fixeds;
    }

    [Serializable]
    public class TurretDatabase
    {
        public List<TurretDefinition> turret;
    }


}