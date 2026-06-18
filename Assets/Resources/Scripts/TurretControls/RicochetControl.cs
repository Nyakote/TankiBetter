using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;


public class RicochetControl : TurretControlBase
{

    #region 

    [Header("Basic Turret Stats")]
    private string turretName = "Ricochet";
    private string turretMod;
    private int price;
    public int minDamage;
    public int maxDamage;
    public float rotationSpeed;
    public float rotationAcceleration;

    [Header("Turret Stats")]
    public float energy_per_shot;
    public float min_damage_range;
    public float max_damage_range;
    public float projectile_speed;
    public float impact_force;
    public float recoil;
    public int energy_capacity;
    public int energy_recharge;
    public int weak_damage_percent;
    public float projectile_radius;
    public float reload_time;
    public int auto_aim_up;
    public int auto_aim_down;

    public float timeBetweenShots = 0.2f;
    public float timepassed = 0f;

    private float currentEnergy;
    private float startReloadTime;
    private float timeBetweenReloads = 0.5f;
    private bool reloadTimeIsSet = false;
    private bool isShooting = false;
    private bool isInitialized = false;

    [Header("References")]
    private ParticleSystem ps;
    private RicochetStatsLoader ricochet;
    private bool isHolding = false;
    private ParticleSystem.Particle[] particles;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem.MainModule mainModule;
    CanvasUI cu;
    #endregion


    public string enemyTag;


    void Start()
    {
        ricochet = GetComponent<RicochetStatsLoader>();
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
        var (turretStats, turretFixed) = ricochet.GetFullStats(turretName, turretMod);
        price = turretStats.price;
        minDamage = turretStats.min_damage;
        maxDamage = turretStats.max_damage;
        energy_per_shot = turretStats.energy_per_shot;
        min_damage_range = turretStats.min_damage_range;
        max_damage_range = turretStats.max_damage_range;
        projectile_speed = turretStats.projectile_speed;
        impact_force = turretStats.impact_force;
        recoil = turretStats.recoil;
        rotationSpeed = turretStats.rotation_speed;
        rotationAcceleration = turretStats.rotation_acceleration;
        energy_capacity = turretFixed.energy_capacity;
        energy_recharge = turretFixed.energy_recharge/10;
        weak_damage_percent = turretFixed.weak_damage_percent;
        projectile_radius = turretFixed.projectile_radius;
        reload_time = turretStats.reload_time;
        auto_aim_up = turretFixed.auto_aim_up;
        auto_aim_down = turretFixed.auto_aim_down;

        currentEnergy = energy_capacity;
        var pjspeed = ps.main.startSpeed;
        pjspeed = projectile_speed;    
        rotationSpeed = rotationSpeed / 10f;
        Transform root = transform.parent.parent.parent;
        enemyTag = root.CompareTag("Blue") ? "Red" : "Blue";
        isInitialized = true;
    }


    public void OnShoot(InputValue value)
    {
        if (transform.parent.parent.parent.CompareTag("Player"))
        {
            if (currentEnergy >= energy_per_shot)
            {
                isHolding = value.isPressed;

                if (ps == null)
                {
                    Debug.LogError("ParticleSystem 'ps' is null in OnShoot!");
                    return;
                }
                isShooting = true;
                ParticleSystem.CollisionModule collision = ps.collision;
                collision.enabled = true;
                collision.collidesWith = LayerMask.GetMask("Enemy", "Map");
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
        health.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(minDamage, maxDamage)));

        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);
        int numParticlesAlive = ps.GetParticles(particles);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 collisionPoint = collisionEvents[i].intersection;

            int closestIndex = -1;
            float closestDist = float.MaxValue;

            for (int j = 0; j < numParticlesAlive; j++)
            {
                float dist = Vector3.Distance(particles[j].position, collisionPoint);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = j;
                }
            }

            if (closestIndex >= 0 && closestDist < 0.5f)
            {
                particles[closestIndex].remainingLifetime = -1f;
            }
        }

        ps.SetParticles(particles, numParticlesAlive);
    }




    public override float GetRotateSpeed() => rotationSpeed;
    public override float MinDamage() => minDamage;
    public override float MaxDamage() => maxDamage;
    public override float EnergyConsumption() => energy_per_shot;
    public override float EnergyCapacity() => energy_capacity;
    public override float ReloadTime() => energy_recharge;
    public override float TimeBetweenShots() => timeBetweenShots;
}
