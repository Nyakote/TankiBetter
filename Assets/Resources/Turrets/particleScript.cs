using UnityEngine;

public class particleScript : MonoBehaviour
{
    private TurretControlBase turretControl;

    void Start()
    {
        turretControl = GetComponentInParent<TurretControlBase>();

        if (turretControl == null)
            Debug.LogWarning("No TurretControlBase found on parent.");
    }

    void OnParticleCollision(GameObject other)
    {
        if (turretControl != null)
            turretControl.HandleParticleCollision(other);
    }
}
