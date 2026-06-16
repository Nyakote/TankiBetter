using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FirebirdControl : TurretControlBase
{
    [Header("Basic Turret Stats")]
    private string turretName = "Firebird";
    private string turretMod;
    private int price;
    public int damage;
    public float rotationSpeed;
    public float rotationAcceleration;

    [Header("Turret Stats")]
    public float reload_time;
    public float burn_damage;
    public float min_damage_range;
    public float max_damage_range;
    public int energy_capacity;
    public int energy_consumption;
    public int weak_damage_percent;
    public float cone_angle;
    public ParticleSystem fireParticles;

    public float currentEnergy;
    private float startReloadTime;
    private float timeBetweedReloads = 0.5f;
    private bool isShooting = false;
    private bool isStartTimeSet = false;



    [Header("References")]
    private ParticleSystem ps;
    private FirebirdStatsLoader firebird;
    CanvasUI cu;
    public string enemyTag;

    private List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
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
        firebird = GetComponent<FirebirdStatsLoader>();
        ps = GetComponentInChildren<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        

        turretMod = transform.parent.parent.parent.name.Split(' ')[3];
        var (turretStats, turretFixed) = firebird.GetFullStats(turretName, turretMod);
        price = turretStats.price;
        damage = turretStats.damage;
        rotationSpeed = turretStats.rotation_speed;
        rotationAcceleration = turretStats.rotation_acceleration;

        reload_time = turretStats.reload_time;
        burn_damage = turretStats.burn_damage;
        min_damage_range = turretStats.min_damage_range;
        max_damage_range = turretStats.max_damage_range;
        energy_capacity = turretFixed.energy_capacity;
        energy_consumption = turretFixed.energy_consumption;
        weak_damage_percent = turretFixed.weak_damage_percent;
        cone_angle = turretFixed.cone_angle;
        currentEnergy = energy_capacity;

        var shape = ps.shape;
        shape.angle = cone_angle;
        rotationSpeed /= 10f;

        Transform root = transform.parent.parent.parent;
        enemyTag = root.CompareTag("Blue") ? "Red" : "Blue";


    }

    public void OnShoot(InputValue value)
    {
        if (transform.parent.parent.parent.tag == "Player")
        {
            if (value.isPressed && currentEnergy > energy_consumption*Time.deltaTime)
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

        int eventCount = fireParticles.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < eventCount; i++)
        {
            if (other.CompareTag(enemyTag))
            {
                HealthComponent health = other.GetComponent<HealthComponent>();
                if (health != null)
                {
                    health.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(damage*Time.deltaTime, damage * Time.deltaTime)));
                }
            }

            RemoveParticleAt(collisionEvents[i].intersection);
        }
    }

    private void RemoveParticleAt(Vector3 hitPosition)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[fireParticles.main.maxParticles];
        int count = fireParticles.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 particleWorldPos = fireParticles.transform.TransformPoint(particles[i].position);
            if (Vector3.Distance(particleWorldPos, hitPosition) < 0.1f)
            {
                particles[i].remainingLifetime = 0f; 
            }
        }

        fireParticles.SetParticles(particles, count);
    }

    public override float GetRotateSpeed() => rotationSpeed;
    public override float MinDamage() => damage * Time.deltaTime;
    public override float MaxDamage() => damage * Time.deltaTime;
    public override float EnergyConsumption() => energy_consumption;
    public override float EnergyCapacity() => energy_capacity;
    public override float ReloadTime() => reload_time;
    public override float TimeBetweenShots() => 0f;
}
