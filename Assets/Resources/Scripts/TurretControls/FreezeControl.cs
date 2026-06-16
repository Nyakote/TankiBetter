using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class FreezeControl : TurretControlBase
{

    #region 

    [Header("Basic Turret Stats")]
    private string turretName = "Freeze";
    private string turretMod;
    private int price;
    public int damage;
    public float rotationSpeed;
    public float rotationAcceleration;

    [Header("Turret Stats")]
    public float reload_time;
    public float min_damage_range;
    public float max_damage_range;
    public int energy_capacity;
    public int freeze_duration;
    public int energy_consumption;
    public int weak_damage_percent;
    public float cone_angle;

    [Header("Visuals")]
    public float currentEnergy;
    private float startReloadTime;
    private float timeBetweedReloads = 0.5f;
    private bool isShooting = false;
    private bool isStartTimeSet = false;

    [Header("References")]
    private ParticleSystem ps;
    private FreezeStatsLoader freeze;
    CanvasUI cu;
    #endregion
    public string enemyTag;





    void Start()
    {
        freeze = GetComponent<FreezeStatsLoader>();
        ps = GetComponentInChildren<ParticleSystem>();
        Invoke("StatsSetter", 0.2f);

    }
  
    private void StopShooting()
    {
        isShooting = false;
        startReloadTime = Time.time;
        var em = ps.emission;
        em.enabled = false;
    }
    private void StatsSetter()
    { 
        turretMod = transform.parent.parent.parent.name.Split(' ')[3];
        var (turretStats, turretFixed) = freeze.GetFullStats(turretName, turretMod);
        price = turretStats.price;
        damage = turretStats.damage;
        rotationSpeed = turretStats.rotation_speed;
        rotationAcceleration = turretStats.rotation_acceleration;

        reload_time = turretStats.reload_time;
        min_damage_range = turretStats.min_damage_range;
        max_damage_range = turretStats.max_damage_range;
        energy_capacity = 1000;
        energy_consumption = turretFixed.energy_consumption;
        weak_damage_percent = turretFixed.weak_damage_percent;
        cone_angle = turretFixed.cone_angle;
        currentEnergy = 1000f;

        var shape = ps.shape;
        shape.angle = cone_angle;
        rotationSpeed = rotationSpeed / 10f;

        Transform root = transform.parent.parent.parent;
        enemyTag = root.CompareTag("Blue") ? "Red" : "Blue";
    }

    public void OnShoot(InputValue value)
    {
        if (transform.parent.parent.parent.tag == "Player")
        {
            if (value.isPressed && currentEnergy > 0)
            {
                var collision = ps.collision;
                collision.collidesWith = LayerMask.GetMask("Enemy", "Map");
                isShooting = true;
                var em = ps.emission;
                em.enabled = true;
            }
            else
            {
                StopShooting();
            }
        }
    }
    public override void HandleParticleCollision(GameObject other)
    {
        if (!other.CompareTag(enemyTag)) return;


        HealthComponent health = other.GetComponent<HealthComponent>();
        health.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(damage*Time.deltaTime, damage * Time.deltaTime)));

    }

    public override float GetRotateSpeed() => rotationSpeed;
    public override float MinDamage() => damage * Time.deltaTime;
    public override float MaxDamage() => damage * Time.deltaTime;
    public override float EnergyConsumption() => energy_consumption;
    public override float EnergyCapacity() => 1000f;
    public override float ReloadTime() => reload_time;
    public override float TimeBetweenShots() => 0f;
}
