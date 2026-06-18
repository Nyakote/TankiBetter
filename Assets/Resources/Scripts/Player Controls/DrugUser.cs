using UnityEngine;
using UnityEngine.InputSystem;

public class DrugUser : MonoBehaviour
{
    private float timeOfHeal;
    private float healDur = 3f;
    private float healRate;
    private bool isHealing = false;
    
    
    HealthComponent healthComponent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHealing)
        {
            float step = healRate * Time.deltaTime;
            healthComponent.currentHealth += step;
            timeOfHeal -= Time.deltaTime;
            if(timeOfHeal <= 0 || healthComponent.currentHealth >= healthComponent.maxHealth)
            {
                isHealing = false;
                healthComponent.currentHealth = Mathf.Min(healthComponent.currentHealth, healthComponent.maxHealth);
            }

        }

        
    }

    public void OnHealthDrug(InputValue value)
    {
        healRate = (healthComponent.maxHealth * 0.2f) / healDur;
        timeOfHeal = healDur;
        isHealing = true;
    }
}
