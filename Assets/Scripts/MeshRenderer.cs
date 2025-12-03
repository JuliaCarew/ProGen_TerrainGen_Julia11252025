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
        // register with GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.SetTerrainMeshRenderer(this);
        
        // get or add components
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        // apply material 
        if (terrainMaterial != null)
            meshRenderer.material = terrainMaterial;
    }

    public void UpdateMesh(Mesh mesh)
    {
        if (mesh == null)
            return;
        
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