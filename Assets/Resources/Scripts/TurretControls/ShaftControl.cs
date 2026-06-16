using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;


public class ShaftControl : TurretControlBase
{

    #region 

    [Header("Basic Turret Stats")]
    private string turretName = "Shaft";
    private string turretMod;
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


    [Header("Turret Stats")]
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

    [Header("Visuals")]
    private ParticleSystem.Particle[] particles;
    public float currentEnergy;
    private float startReloadTime;
    private float timeBetweedReloads = 0.5f;
    private bool isShooting = false;
    private bool isStartTimeSet = false;
    private bool isInitialized = false;

    [Header("References")]
    private ParticleSystem ps;
    private ShaftStatsLoader shaft;
    CanvasUI cu;
    #endregion
    public string enemyTag;

    void Start()
    {
        shaft = GetComponent<ShaftStatsLoader>();
        ps = GetComponentInChildren<ParticleSystem>();
        particles = new ParticleSystem.Particle[1];
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
        var (turretStats, turretFixed) = shaft.GetFullStats(turretName, turretMod);
        price= turretStats.price;
        scoped_min_damage = turretStats.scoped_min_damage;

        scoped_max_damage = turretStats.scoped_max_damage;
        arcade_min_damage = turretStats.arcade_min_damage;
        arcade_max_damage = turretStats.arcade_max_damage;
        arcade_impact_force = turretStats.arcade_impact_force;
        scoped_impact_force = turretStats.scoped_impact_force;
        recoil = turretStats.recoil;
        arcade_reload_time = turretStats.arcade_reload_time;
        energy_consumption = turretStats.energy_consumption;
        rotation_speed = turretStats.rotation_speed;
        rotation_acceleration = turretStats.rotation_acceleration;
        horizontal_aim_speed = turretStats.horizontal_aim_speed;
        vertical_aim_speed = turretStats.vertical_aim_speed;
        energy_per_shot = turretStats.energy_per_shot;
        energy_recharge = turretStats.energy_recharge;
        piercing_percent = turretStats.piercing_percent;
        energy_capacity = turretFixed.energy_capacity;
        scoped_exit_delay = turretFixed.scoped_exit_delay;
        scoped_entry_delay = turretFixed.scoped_entry_delay;
        scoped_rotation_acceleration = turretFixed.scoped_rotation_acceleration;
        auto_aim_up = turretFixed.auto_aim_up;
        auto_aim_down = turretFixed.auto_aim_down;
        min_foliage_transparency_radius = turretFixed.min_foliage_transparency_radius;
        fov_max = turretFixed.fov_max;
        fov_min = turretFixed.fov_min;
        max_foliage_transparency_radius = turretFixed.max_foliage_transparency_radius;
        slowdown_start_point = turretFixed.slowdown_start_point;
        slowdown_end_point = turretFixed.slowdown_end_point;
        min_slowdown_factor = turretFixed.min_slowdown_factor;

        rotation_speed = rotation_speed / 10f;
        currentEnergy = 1000f;
        Transform root = transform.parent.parent.parent;
        enemyTag = root.CompareTag("Blue") ? "Red" : "Blue";
        isInitialized = true;
    }

    public void OnShoot(InputValue value)
    {
        if (transform.parent.parent.parent.tag == "Player")
        {
            if (currentEnergy >= energy_consumption)
            {
                ParticleSystem.CollisionModule collision = ps.collision;
                collision.enabled = true;
                collision.collidesWith = LayerMask.GetMask("Enemy", "Map");
                ps.Emit(1);
                isShooting = true;
                currentEnergy -= energy_consumption;
                startReloadTime = Time.time;
            }
            else
            {
                StopShooting();
            }
        }
    }
    public override void HandleParticleCollision(GameObject other)
    {
        int aliveCount = ps.GetParticles(particles);
        if (other.CompareTag("Map"))
        {
            
            if (aliveCount > 0)
            {
                particles[0].remainingLifetime = -1f;
                ps.SetParticles(particles, aliveCount);
            }
            return;
        }
        if (!other.CompareTag(enemyTag)) return;


        HealthComponent health = other.GetComponent<HealthComponent>();
        health.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(arcade_min_damage, arcade_max_damage)));
        particles[0].remainingLifetime = -1f;
        ps.SetParticles(particles, aliveCount);
    }
    public override float GetRotateSpeed() => rotation_speed;
    public override float MinDamage() => arcade_min_damage;
    public override float MaxDamage() => arcade_max_damage;
    public override float EnergyConsumption() => energy_consumption/2;
    public override float EnergyCapacity() => energy_capacity;
    public override float ReloadTime() => energy_recharge*2;
    public override float TimeBetweenShots() => 0f;
}
