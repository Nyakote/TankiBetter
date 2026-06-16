using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    CanvasUI CUI;
    // Death event
    public event Action OnDeath;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        CUI.currentHealth = currentHealth;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        OnDeath?.Invoke(); // Notify subscribers
        Destroy(transform.parent.gameObject);
    }

    public void Initialize(float HP)
    {
        currentHealth = HP;
        maxHealth = HP;
        CUI = GetComponentInChildren<CanvasUI>();
        Invoke("CuiPlacer", 0.5f);
    }

    private void CuiPlacer()
    {
        CUI.HealthBar(maxHealth, currentHealth);
    }
}
