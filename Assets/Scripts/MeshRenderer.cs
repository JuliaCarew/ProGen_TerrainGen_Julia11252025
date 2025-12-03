using UnityEngine;

/// Handles mesh rendering and material assignment for generated terrain.
public class TerrainMeshRenderer : MonoBehaviour
{
    [Header("Material Settings")]
    [SerializeField] private Material terrainMaterial;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Awake()
    {
        // get or add components
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
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
        
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        
        // assign mesh
        if (meshFilter != null)
            meshFilter.mesh = mesh;
        
        if (meshCollider != null)
            meshCollider.sharedMesh = mesh;
        
        // ensure material is set and uses vertex colors
        if (meshRenderer != null)
        {
            // vertex color shader
            if (terrainMaterial == null)
            {
                Shader vertexColorShader = Shader.Find("Custom/VertexColor");
                if (vertexColorShader != null)
                    terrainMaterial = new Material(vertexColorShader);
            }
            
            if (terrainMaterial != null)
                meshRenderer.material = terrainMaterial;
        }
    }
    
    public void SetMaterial(Material material)
    {
        terrainMaterial = material;
        if (meshRenderer != null && terrainMaterial != null)
            meshRenderer.material = terrainMaterial;
    }
}