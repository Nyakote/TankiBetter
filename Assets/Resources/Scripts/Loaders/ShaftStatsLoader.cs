using System;
using System.Collections.Generic;
using UnityEngine;

public class ShaftStatsLoader : MonoBehaviour
{
    public TurretDatabase TurretStats;

    private void Awake()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/ShaftSTATS");

        if (jsonText != null)
        {
            TurretStats = JsonUtility.FromJson<TurretDatabase>(jsonText.text);
        }
        else
        {
            Debug.LogError("Couldn't find ShaftSTATS.json in Resources!");
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
        public int scoped_min_damage;
        public int scoped_max_damage;
        public int arcade_min_damage;
        public int arcade_max_damage;
        public float arcade_impact_force;
        public float scoped_impact_force;
        public float recoil;
        public float arcade_reload_time;
        public float energy_consumption;
        public float rotation_speed;
        public float rotation_acceleration;
        public float horizontal_aim_speed;
        public float vertical_aim_speed;
        public float energy_per_shot;
        public float energy_recharge;
        public float piercing_percent;
    }


    [Serializable]
    public class TurretFixed
    {
        public int energy_capacity;
        public float scoped_exit_delay;
        public float scoped_entry_delay;
        public float scoped_rotation_acceleration;
        public int auto_aim_up;
        public int auto_aim_down;
        public int min_foliage_transparency_radius;
        public int max_foliage_transparency_radius;
        public int fov_min;
        public int fov_max;
        public float slowdown_start_point;
        public float slowdown_end_point;
        public float min_slowdown_factor;
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