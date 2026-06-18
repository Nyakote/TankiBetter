using UnityEngine;

public class HullStatsLoader : MonoBehaviour
{
    public HullDatabase HullStats;

    void Awake()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/HullStats");

        if (jsonText != null)
        {
            HullStats = JsonUtility.FromJson<HullDatabase>(jsonText.text);
        }
        else
        {
            Debug.LogError("Couldn't find HullStats.json in Resources!");
        }
    }

    public HullMod GetStats(string hullName, string modLevel)
    {
        HullDefinition hull = HullStats.hull.Find(h => h.name == hullName);
        if (hull == null)
        {
            Debug.LogWarning($"Hull '{hullName}' not found!");
            return null;
        }

        HullMod mod = hull.mod.Find(m => m.tier == modLevel);
        if (mod == null)
        {
            Debug.LogWarning($"Mod '{modLevel}' not found in hull '{hullName}'!");
        }
        return mod;
    }
}
