using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretControl : MonoBehaviour
{
    #region Variables

    [Header("Name/Mod")]
    private GameObject hullGO;
    private GameObject turretGO;

    [Header("Turret Stats")]
    public float price;
    public float minDamage;
    public float maxDamage;
    public float impactForce;
    public float recoil;
    public float reloadTime;
    public float rotationSpeed;
    public float criticalChance;
    public float criticalDamage;
    public float minDamageRange;
    public float maxDamageRange;

    [Header("Turret Control")]
    private float inputTurret;
    private bool isCentering = false;
    private float currentTurretYaw = 0f;
    private float startLocalYaw = 0f;

    private float hullYaw;
    private float currentYaw;
    private string turretName;
    private string turretMod;

    private TurretControlBase trBase;
    private TurretControlBase trBaseValues;

    // Optional clamp
    // public float maxYawOffset = 90f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        Invoke(nameof(CalculateInitialYawOffset), 0.5f);
    }

    private void Update()
    {
        if (inputTurret != 0)
        {
            TurretTurning();
            isCentering = false;
        }
       else  if (isCentering)
            TurretCentering();
       else {
            KeepTurretStraight();
        }

    }

    public void OnTurretTurning(InputValue value)
    {
        inputTurret = value.Get<float>();
    }

    public void OnTurretCentering(InputValue value)
    {
        isCentering = value.isPressed;
    }

    #endregion

    #region Custom Methods

    private void CalculateInitialYawOffset()
    {
        currentYaw = turretGO.transform.eulerAngles.y;
        hullYaw = hullGO.transform.eulerAngles.y;
        startLocalYaw = Mathf.DeltaAngle(hullYaw, currentYaw);
        currentTurretYaw = startLocalYaw;
    }

    private void TurretCentering()
    { 
        float angle = Vector3.SignedAngle(hullGO.transform.forward, turretGO.transform.forward, Vector3.up);
        float newAngle = Mathf.MoveTowardsAngle(angle, 0f, rotationSpeed * Time.deltaTime);

        if (Mathf.Abs(newAngle) < 0.1f)
        {
            isCentering = false;
            newAngle = 0f;
        }
        currentTurretYaw = newAngle;
        turretGO.transform.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
    }

    private void TurretTurning()
    {
        currentTurretYaw += inputTurret * rotationSpeed/1.05f * Time.deltaTime;
        turretGO.transform.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
    }

    private void KeepTurretStraight()
    {
        turretGO.transform.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
    }

    public void Initialize(GameObject hullGO_import, GameObject turretGO_import, string turretName_import, string turretMod_import)
    {
        hullGO = hullGO_import;
        turretGO = turretGO_import;
        turretName = turretName_import;
        turretMod = turretMod_import;
        Invoke("Setter", 0.6f);
        
    }

    private void Setter()
    {
        trBase = turretGO.GetComponent<TurretControlBase>();
        if (trBase == null)
        {
            Debug.LogError("TurretControlBase not found on turret GameObject.");
            return;
        }

        trBaseValues = trBase.GetTurretControl(turretName, turretMod);
        rotationSpeed = trBaseValues.GetRotateSpeed()*5;
    }
    #endregion
}
