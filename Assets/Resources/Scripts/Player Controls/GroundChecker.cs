using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    #region Variables

    [Header("Ground Check bool")]
    public bool RearCheck = false;
    public bool FrontCheck = false;
    public bool MidCheck = false;

    [Header("Hull length")]
    private float halfZ;
    private float maxOffset = 0.7f;

    [Header("Raycast Placements/offset")]
    private float RearCheckPlacement;
    private float FrontCheckPlacement;
    private float MidCheckPlacement;
    private Vector3 rearRayOrigin, frontRayOrigin, midRayOrigin;
    private float coliderXCenter;

    [Header("References")]
    private BoxCollider cl;
    private LayerMask groundMask;

    [Header("Raycast Settings")]
    private float rayLength = 0.5f;
    private float rayStartHeightOffset = 0.8f;
    private float rayRadius = 0.75f; 

    #endregion

    #region UnityMethods

    private void Start()
    {
        Invoke("Something", 0.7f);
    }

    private void FixedUpdate()
    {

        RearCheck = RayCheckAtOffset(RearCheckPlacement, out rearRayOrigin);
        FrontCheck = RayCheckAtOffset(FrontCheckPlacement, out frontRayOrigin);
        MidCheck = RayCheckAtOffset(MidCheckPlacement, out midRayOrigin);

        Drawer(RearCheck,rearRayOrigin);
        Drawer(FrontCheck, frontRayOrigin);
        Drawer(MidCheck, midRayOrigin);

    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(rearRayOrigin, rayRadius);
        Gizmos.DrawWireSphere(midRayOrigin, rayRadius);
        Gizmos.DrawWireSphere(frontRayOrigin, rayRadius);
    }

    #endregion

    #region CustomMethods

    private void Something()
    {
        
        cl = GetComponent<BoxCollider>();
        halfZ = cl.bounds.size.z / 2;
        coliderXCenter = cl.center.x;
        RayCastPlacer();
        groundMask = LayerMask.GetMask("Map");
    }

    private void RayCastPlacer()
    {
        FrontCheckPlacement = halfZ - maxOffset;
        RearCheckPlacement = -halfZ + maxOffset;
        MidCheckPlacement = 0.0f;
    }

    private bool RayCheckAtOffset(float zOffset, out Vector3 origin)
    {
        Vector3 localOffset = new Vector3(coliderXCenter, rayStartHeightOffset, zOffset);
        origin = transform.TransformPoint(localOffset);
        return Physics.SphereCast(origin, rayRadius, Vector3.down, out RaycastHit hit, rayLength, groundMask);
    }


    private void Drawer(bool isIt, Vector3 place)
    {
        Debug.DrawRay(place, Vector3.down * rayLength, isIt ? Color.green : Color.red);

    }
    #endregion
}
