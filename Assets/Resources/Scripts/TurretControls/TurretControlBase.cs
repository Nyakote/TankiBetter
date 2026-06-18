using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class TurretControlBase : MonoBehaviour
{
   FirebirdControl FirebirdControl;
   FreezeControl FreezeControl;
   IsidaControl IsidaControl;
   RailgunControl RailgunControl;
   RicochetControl RicochetControl;
   ShaftControl ShaftControl;
   SmokyControl SmokyControl;
   ThunderControl ThunderControl;
   TwinsControl TwinsControl;
    
    private Dictionary<string, TurretControlBase> turretControls;

    private void Awake()
    {
        FirebirdControl = GetComponent<FirebirdControl>();
        FreezeControl = GetComponent<FreezeControl>();
        IsidaControl = GetComponent<IsidaControl>();
        RailgunControl = GetComponent<RailgunControl>();
        RicochetControl = GetComponent<RicochetControl>();
        ShaftControl = GetComponent<ShaftControl>();
        SmokyControl = GetComponent<SmokyControl>();
        ThunderControl = GetComponent<ThunderControl>();
        TwinsControl = GetComponent<TwinsControl>();

        turretControls = new Dictionary<string, TurretControlBase>()
        {
            { "Firebird", FirebirdControl },
            { "Freeze", FreezeControl },
            { "Isida", IsidaControl },
            { "Railgun", RailgunControl },
            { "Ricochet", RicochetControl },
            { "Shaft", ShaftControl },
            { "Smoky", SmokyControl },
            { "Thunder", ThunderControl },
            { "Twins", TwinsControl }
        };
    }

    public TurretControlBase GetTurretControl(string turretName, string mod)
    {
        if (turretControls == null)
        {
            Debug.LogError("Turret controls dictionary not initialized!");
        }

        if (turretControls.TryGetValue(turretName, out var control))
        {
            return control;
        }
        else
        {
            Debug.LogWarning($"Turret control for '{turretName}' not found.");
            return null;
        }
    }
    public abstract float GetRotateSpeed();
    public abstract float MinDamage();
    public abstract float MaxDamage();
    public abstract float ReloadTime();
    public abstract float EnergyCapacity();
    public abstract float EnergyConsumption();
    public abstract float TimeBetweenShots();
    public abstract void HandleParticleCollision(GameObject other);
    

}
