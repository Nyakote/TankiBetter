/*using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PickLoader : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;
    BoxCollider playerCollider;
    HealthComponent healthComponent;
    public string hullName;
    public string turretName;
    public string hullMod;
    public string turretMod;
    public string paint;
    public Material TankPainter;
    
    GameObject hullInstance;
    GameObject turretInstance;

    TurretControl trControl;
    CameraControl cmControl;
    public GameObject canvas;
    CanvasUI cu;

    void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        playerCollider = playerRoot.GetComponent<BoxCollider>();
        trControl = GetComponent<TurretControl>();
        cmControl = GameObject.Find("Camera").GetComponent<CameraControl>();
        cu = canvas.GetComponent<CanvasUI>();

        hullName = PlayerPrefs.GetString("SelectedHull", "Wasp");
        turretName = PlayerPrefs.GetString("SelectedTurret", "Smoky");
        hullMod = PlayerPrefs.GetString("SelectedHullMod", "M0");
        turretMod = PlayerPrefs.GetString("SelectedTurretMod", "M0");
        paint = PlayerPrefs.GetString("SelectedPaint", "green");


        GameObject hullPrefab = Resources.Load<GameObject>("Hulls/" + hullName);
        GameObject turretPrefab = Resources.Load<GameObject>("Turrets/" + turretName);
        transform.name = transform.name + " " + hullName + " " + turretName + " " + hullMod + " " + turretMod;
        if (hullPrefab && turretPrefab)
        {
            Quaternion originalRotation = playerRoot.rotation;
            playerRoot.rotation = Quaternion.identity;

            hullInstance = Instantiate(hullPrefab, playerRoot.position, playerRoot.rotation);
            hullInstance.transform.SetParent(playerRoot);
            hullInstance.transform.localPosition = Vector3.zero;
            hullInstance.transform.localRotation = Quaternion.identity;

            Transform turretMount = hullInstance.transform.Find("mount");

            if (turretMount != null)
            {
                turretInstance = Instantiate(turretPrefab);


                turretInstance.transform.SetParent(turretMount);
                turretInstance.transform.localPosition = Vector3.zero;
                turretInstance.transform.localRotation = Quaternion.identity;
                Renderer[] allRenderers = hullInstance.GetComponentsInChildren<Renderer>(true).Concat(turretInstance.GetComponentsInChildren<Renderer>(true)).ToArray();

                Bounds totalBounds = new Bounds();
                bool hasBounds = false;

                foreach (Renderer r in allRenderers)
                {
                    if (!r.enabled || !r.gameObject.activeInHierarchy) continue;
                    if (r is ParticleSystemRenderer) continue;
                    if (r.bounds.size == Vector3.zero) continue;
                    if (!hasBounds)
                    {
                        totalBounds = r.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        totalBounds.Encapsulate(r.bounds);
                    }
                }

                if (hasBounds)
                {
                    playerCollider.center = playerRoot.InverseTransformPoint(totalBounds.center);
                    playerCollider.size = totalBounds.size;
                }

                playerRoot.rotation = originalRotation;
            }
           
            foreach (Transform t in playerRoot.GetComponentsInChildren<Transform>())
            {
                t.gameObject.tag = "Player";
            }
        }

        Texture2D camoTex = Resources.Load<Texture2D>($"Paints/Skins/{paint}_image");
        Texture2D hullElements = Resources.Load<Texture2D>($"Hulls/Skins/{hullName.ToLower()}_{hullMod.ToLower()}");
        Texture2D turretElements = Resources.Load<Texture2D>($"Turrets/Skins/{turretName.ToLower()}_{turretMod.ToLower()}");

        Renderer turretSkin = turretInstance.GetComponentInChildren<Renderer>();
        Renderer hullSkin = hullInstance.GetComponentInChildren<Renderer>();

        Material hullMaterial = new Material(TankPainter);
        Material turretMaterial = new Material(TankPainter);

        hullMaterial.SetTexture("_Base", hullElements);
        hullMaterial.SetTexture("_Layer1", camoTex);

        turretMaterial.SetTexture("_Base", turretElements);
        turretMaterial.SetTexture("_Layer1", camoTex);
        


        turretSkin.material = turretMaterial;
        hullSkin.material = hullMaterial;

        trControl.Initialize(hullInstance,turretInstance , turretName, turretMod);
        cmControl.Initialize(hullInstance, turretInstance);
        cu.Initialize(turretInstance, turretName,turretMod);
        

    }
}
*/