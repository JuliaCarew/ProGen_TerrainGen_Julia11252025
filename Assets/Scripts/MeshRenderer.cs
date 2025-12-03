using UnityEngine;

public class TerrainMeshRenderer : MonoBehaviour
{
    [Header("Material Settings")]
    [SerializeField] private Material terrainMaterial;
    
    private MeshFilter meshFilter;
    private UnityEngine.MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Awake()
    {
        // get or add components
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        
        meshRenderer = GetComponent<UnityEngine.MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<UnityEngine.MeshRenderer>();
        
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        
        // apply material 
        if (terrainMaterial != null)
            meshRenderer.material = terrainMaterial;
    }

    public void UpdateMesh(Mesh mesh)
    {
        if (mesh == null)
            return;
        
        // ensure components exist
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        
        if (meshCollider == null)
        {
            meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        
        // assign mesh
        if (meshFilter != null)
            meshFilter.mesh = mesh;
        
        if (meshCollider != null)
            meshCollider.sharedMesh = mesh;
    }
    
    public void SetMaterial(Material material)
    {
        terrainMaterial = material;
        if (meshRenderer != null && terrainMaterial != null)
            meshRenderer.material = terrainMaterial;
    }
}