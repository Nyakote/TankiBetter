using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;


public class TwinsControl : TurretControlBase
{

    #region 

    [Header("Basic Turret Stats")]
    public string turretName = "Twins";
    public string turretMod;
    public int price;
    public float min_damage;
    public float max_damage;
    public float impact_force;
    public float recoil;
    public float rotation_speed;
    public float rotation_acceleration;
    public float min_damage_range;
    public float projectile_speed;
    public float weak_damage_percent;
    public float reload_time;
    public float max_damage_range;
    public float projectile_radius;

    [Header("Turret Stats")]
    public int auto_aim_up;
    public int auto_aim_down;
    public float timeBetweenShots = 0.2f;
    public float timepassed = 0f;
    bool right = true;
    [SerializeField] Transform RightC;
    [SerializeField] Transform LeftC;


    private float currentEnergy;
    private float startReloadTime;
    private float timeBetweenReloads = 0f;
    private bool reloadTimeIsSet = false;
    private bool isShooting = false;
    private bool isInitialized = false;
    private float energy_capacity;
    private float energy_per_shot;

    [Header("References")]

    private ParticleSystem ps;
    private TwinsStatsLoader twins;
    private bool isHolding = false;
    #endregion
    public string enemyTag;
    private ParticleSystem.Particle[] particles;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem.MainModule mainModule;
    CanvasUI cu;


    void Start()
    {
        twins = GetComponent<TwinsStatsLoader>();
        ps = GetComponentInChildren<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
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
        var (turretStats, turretFixed) = twins.GetFullStats(turretName, turretMod);
        price = turretStats.price;
        min_damage = turretStats.min_damage;
        max_damage = turretStats.max_damage;
        impact_force = turretStats.impact_force;
        recoil = turretStats.recoil;
        rotation_speed = turretStats.rotation_speed;
        rotation_acceleration = turretStats.rotation_acceleration;
        min_damage_range = turretStats.min_damage_range;
        projectile_speed = turretStats.projectile_speed;
        weak_damage_percent = turretStats.weak_damage_percent;
        reload_time = turretStats.reload_time;
        max_damage_range = turretStats.max_damage_range;
        projectile_radius = turretStats.projectile_radius;
        auto_aim_up = turretFixed.auto_aim_up;
        auto_aim_down = turretFixed.auto_aim_down;
        energy_per_shot = 1000f;
        energy_capacity = 1000;
        currentEnergy = 1000f;


        var pjspeed = ps.main.startSpeed;
        pjspeed = projectile_speed;
        rotation_speed = rotation_speed / 10f;

        Transform root = transform.parent.parent.parent;
        enemyTag = root.CompareTag("Blue") ? "Red" : "Blue";
        isInitialized = true;
    }


    public void OnShoot(InputValue value)
    {
        if (!(transform.parent.parent.parent.tag == "Player")) return;
        isHolding = value.isPressed;

        if (isHolding && currentEnergy >= energy_per_shot)
        {
            var em = ps.emission;
            em.enabled = true;

            var collision = ps.collision;
            collision.enabled = true;
            collision.collidesWith = LayerMask.GetMask("Enemy", "Map");
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
        if (aliveCount > 0)
        {
            particles[0].remainingLifetime = -1f;
            ps.SetParticles(particles, aliveCount);
        }
    }
    public override float GetRotateSpeed() => rotation_speed;
    public override float MinDamage() => min_damage;
    public override float MaxDamage() => max_damage;
    public override float EnergyConsumption() => 1000f;
    public override float EnergyCapacity() => 1000f;
    public override float ReloadTime() => reload_time;
    public override float TimeBetweenShots() => 0f;
}
