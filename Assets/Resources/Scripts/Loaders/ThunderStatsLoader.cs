using System;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStatsLoader : MonoBehaviour
{
    public TurretDatabase TurretStats;

    private void Awake()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/ThunderSTATS");

        if (jsonText != null)
        {
            TurretStats = JsonUtility.FromJson<TurretDatabase>(jsonText.text);
        }
        else
        {
            Debug.LogError("Couldn't find ThunderSTATS.json in Resources!");
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
        public float splash_impact_force;
        public float recoil;
        public float reload_time;
        public float rotation_speed;
        public float rotation_acceleration;
        public float min_damage_range;
        public float max_damage_range;
    }

    [Serializable]
    public class TurretFixed
    {
        public int weak_damage_percent;
        public int max_damage_radius;
        public int min_damage_radius;
        public int weak_damage_percent_splash;
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