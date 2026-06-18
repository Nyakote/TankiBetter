using System;
using System.Collections.Generic;
using UnityEngine;

public class FreezeStatsLoader : MonoBehaviour
{
    public TurretDatabase TurretStats;

    private void Awake()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/FreezeSTATS");

        if (jsonText != null)
        {
            TurretStats = JsonUtility.FromJson<TurretDatabase>(jsonText.text);
        }
        else
        {
            Debug.LogError("Couldn't find FreezeSTATS.json in Resources!");
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
        public int damage;
        public float rotation_speed;
        public float rotation_acceleration;
        public float reload_time;
        public float min_damage_range;
        public float max_damage_range;
    }


    [Serializable]
    public class TurretFixed
    {
        public int freeze_duration;
        public int energy_capacity;
        public int energy_consumption;
        public int weak_damage_percent;
        public float cone_angle;
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