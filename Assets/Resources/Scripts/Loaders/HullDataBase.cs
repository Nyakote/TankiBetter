using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HullMod
{
    public string tier;   
    public int HP;
    public float SPD;
    public float ROTSPD;
    public int WEIGHT;
    public int PWR;
}

[Serializable]
public class HullDefinition
{
    public string name;        
    public List<HullMod> mod;
}

[Serializable]
public class HullDatabase
{
    public List<HullDefinition> hull;
}
