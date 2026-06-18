using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        HealthComponent healthComponent;
        healthComponent = other.gameObject.GetComponent<HealthComponent>();
        healthComponent.TakeDamage(9999);
    }
}
