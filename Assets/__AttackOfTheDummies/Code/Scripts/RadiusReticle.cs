using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RadiusReticle : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 0.05f;
    [SerializeField] private float maxRaycastDistance = 10f;
    [SerializeField] private LayerMask groundMask;

    private MaterialPropertyBlock matBlock;
    private MeshRenderer meshRenderer;
    private Camera cam;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        matBlock = new MaterialPropertyBlock();
        cam = Camera.main;
    }

    public void SetPosition(Vector2 worldPos)
    {
        Vector3 rayOrigin = new Vector3(worldPos.x, maxRaycastDistance, worldPos.y);
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, maxRaycastDistance * 2f, groundMask))
        {
            transform.position = hit.point + hit.normal * hoverHeight;
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(cam.transform.forward, hit.normal), hit.normal);

            if (!meshRenderer.enabled)
                meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    public void SetRadius(float radius)
    {
        transform.localScale = Vector3.one * radius * 2f; // Mesh has radius 0.5
        meshRenderer.GetPropertyBlock(matBlock);
        matBlock.SetFloat("_Radius", radius);
        meshRenderer.SetPropertyBlock(matBlock);
    }
}