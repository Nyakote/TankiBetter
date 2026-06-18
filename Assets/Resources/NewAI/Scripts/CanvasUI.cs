using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasUI : MonoBehaviour
{

    public float maxHealth;
    public float currentHealth;

    [SerializeField] Image healthBar;
    public float originalWidth;

    [SerializeField] Image energyBar;
    public float originalEnergWidth;

    public float currentEnergy;
    public float maxEnergy = 1000;
    
    public bool isInitialized = false;
    public bool isInitialized1 = false;




    void Update()
    {
        if (!isInitialized && !isInitialized1) return;

        float healthPercent = currentHealth / maxHealth;
        float newSize = Mathf.Clamp(healthPercent * originalWidth, 0, originalWidth);

        RectTransform rt = healthBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(newSize, rt.sizeDelta.y);

        float energyPercent = currentEnergy / maxEnergy;
        float newEnergySize = Mathf.Clamp(energyPercent * originalEnergWidth, 0, originalEnergWidth);

        RectTransform eb = energyBar.GetComponent<RectTransform>();
        eb.sizeDelta = new Vector2(newEnergySize, rt.sizeDelta.y);
    }



    public void HealthBar(float maxHp, float currentHp)
    {
        RectTransform rt = healthBar.GetComponent<RectTransform>();
        originalWidth = rt.sizeDelta.x;
        RectTransform eb = energyBar.GetComponent<RectTransform>();
        originalEnergWidth = eb.sizeDelta.x;

        maxHealth = maxHp;
        currentHealth = currentHp;
        isInitialized = true;
    }



    public void EnergyBar(float currentEnergyIncome)
    {
        currentEnergy = currentEnergyIncome;
    }
}
