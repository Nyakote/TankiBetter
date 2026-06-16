using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;


public class RailgunControl : TurretControlBase
{

    #region 

    [Header("Basic Turret Stats")]
    private string turretName = "Railgun";
    private string turretMod;
    private int price;
    public int minDamage;
    public int maxDamage;
    public float rotationSpeed;
    public float rotationAcceleration;

    [Header("Turret Stats")]
    public float impact_force;
    public float recoil;
    public float reload_time;
    public float charge_time;
    public float delay;
    public float piercing_damage_percent;
    public int auto_aim_up;
    public int auto_aim_down;
    public float energy_consumption;
    public int energy_capacity;

    //private float startChargingTime;
    public float currentEnergy;
    private float startReloadTime;
    private float timeBetweedReloads = 0.5f;
    private bool isShooting = false;
    private bool isStartTimeSet = false;

    [Header("Visuals")]
    private ParticleSystem.Particle[] particles;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem.MainModule mainModule;

    [Header("References")]
    private ParticleSystem ps;
    private RailgunStatsLoader railgun;
    CanvasUI cu;
    #endregion
    public string enemyTag;


    void Start()
    {
        railgun = GetComponent<RailgunStatsLoader>();
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
        var (turretStats, turretFixed) = railgun.GetFullStats(turretName, turretMod);
        price = turretStats.price;
        minDamage = turretStats.min_damage;
        maxDamage = turretStats.max_damage;
        rotationSpeed = turretStats.rotation_speed;
        rotationAcceleration = turretStats.rotation_acceleration;
        recoil = turretStats.recoil;
        impact_force = turretStats.impact_force;
        reload_time = turretStats.reload_time;
        charge_time = turretStats.charge_time;
        delay = turretStats.delay;
        piercing_damage_percent = turretStats.piercing_damage_percent;
        auto_aim_up = turretFixed.auto_aim_up;
        auto_aim_down = turretFixed.auto_aim_down;
        energy_consumption = 1000f;
        energy_capacity = 1000;
        currentEnergy = 1000f;


        rotationSpeed = rotationSpeed / 10f;

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
        health.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(minDamage, maxDamage)));
        if (aliveCount > 0)
        {
            particles[0].remainingLifetime = -1f;
            ps.SetParticles(particles, aliveCount);
        }
    }
    public override float GetRotateSpeed() => rotationSpeed;
    public override float MinDamage() => minDamage;
    public override float MaxDamage() => maxDamage;
    public override float EnergyConsumption() => 1000f;
    public override float EnergyCapacity() => 1000f;
    public override float ReloadTime() => reload_time;
    public override float TimeBetweenShots() => 0f;
}
