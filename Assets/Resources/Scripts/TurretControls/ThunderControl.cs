using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;


public class ThunderControl : TurretControlBase
{

    #region 

    [Header("Basic Turret Stats")]
    private string turretName = "Thunder";
    private string turretMod;
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
    [Header("Turret Stats")]
    public int weak_damage_percent;
    public int max_damage_radius;
    public int min_damage_radius;
    public int weak_damage_percent_splash;
    public int auto_aim_up;
    public int auto_aim_down;

    [Header("Visuals")]
    public float energy_consumption;
    public int energy_capacity;
    public float currentEnergy;
    private float startReloadTime;
    private float timeBetweedReloads = 0.5f;
    private bool isShooting = false;
    private bool isStartTimeSet = false;

    [Header("References")]
    private ParticleSystem ps;
    private ThunderStatsLoader thunder;
    private ParticleSystem.Particle[] particles;
    CanvasUI cu;
    #endregion
    public string enemyTag;

    void Start()
    {
        thunder = GetComponent<ThunderStatsLoader>();
      
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
        var (turretStats, turretFixed) = thunder.GetFullStats(turretName, turretMod);
        price = turretStats.price;
        min_damage = turretStats.min_damage;
        max_damage = turretStats.max_damage;
        impact_force = turretStats.impact_force;
        splash_impact_force = turretStats.splash_impact_force;
        recoil = turretStats.recoil;
        reload_time = turretStats.reload_time;
        rotation_speed = turretStats.rotation_speed;
        rotation_acceleration = turretStats.rotation_acceleration;
        min_damage_range = turretStats.min_damage_range;
        max_damage_range = turretStats.max_damage_range;
        weak_damage_percent = turretFixed.weak_damage_percent;
        max_damage_radius = turretFixed.max_damage_radius;
        min_damage_radius = turretFixed.min_damage_radius;
        weak_damage_percent_splash = turretFixed.weak_damage_percent_splash;
        auto_aim_up = turretFixed.auto_aim_up;
        auto_aim_down = turretFixed.auto_aim_down;
        energy_consumption = 1000f;
        energy_capacity = 1000;
        currentEnergy = 1000f;
        rotation_speed = rotation_speed / 10f;

        Transform root = transform.parent.parent.parent;
        enemyTag = root.CompareTag("Blue") ? "Red" : "Blue";
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
        health.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(min_damage, max_damage)));
        particles[0].remainingLifetime = -1f;
        ps.SetParticles(particles, aliveCount);
    }
    public override float GetRotateSpeed() => rotation_speed;
    public override float MinDamage() => min_damage;
    public override float MaxDamage() => max_damage;
    public override float EnergyConsumption() => 1000f;
    public override float EnergyCapacity() => 1000f;
    public override float ReloadTime() => reload_time;
    public override float TimeBetweenShots() => 0f;
}
