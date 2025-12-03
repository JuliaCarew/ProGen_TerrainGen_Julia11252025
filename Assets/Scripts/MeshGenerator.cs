using System.Collections;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    #region Variables

    [Header("Terrain Settings")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField] private float scale = 5f;
    [SerializeField] private float heightMultiplier = 10f;
    
    [Header("Perlin Noise Settings")]
    [SerializeField] private float noiseScale = 0.3f;
    [SerializeField] private Vector2 offset = Vector2.zero;
    
    private MeshGen meshSimplify;
    private TerrainMeshRenderer terrainMeshRenderer;
    private Coroutine currentGenerationCoroutine;

    #endregion
    void Start()
    {
        // get or add components
        meshSimplify = GetComponent<MeshGen>();
        if (meshSimplify == null)
            meshSimplify = gameObject.AddComponent<MeshGen>();
        
        terrainMeshRenderer = GetComponent<TerrainMeshRenderer>();
        if (terrainMeshRenderer == null)
            terrainMeshRenderer = gameObject.AddComponent<TerrainMeshRenderer>();
        
        // set mesh size from terrain settings
        meshSimplify.SetSize(width, height);
        
        // generate terrain mesh 
        StartCoroutine(GenerateTerrain());
    }
    
    void Awake()
    {
        // ensure TerrainMeshRenderer is initialized 
        terrainMeshRenderer = GetComponent<TerrainMeshRenderer>();
        if (terrainMeshRenderer == null)
            terrainMeshRenderer = gameObject.AddComponent<TerrainMeshRenderer>();
    }

    IEnumerator GenerateTerrain()
    {
        // Ensure meshSimplify exists
        if (meshSimplify == null)
        {
            meshSimplify = GetComponent<MeshGen>();
            if (meshSimplify == null)
                meshSimplify = gameObject.AddComponent<MeshGen>();
        }
        
        if (terrainMeshRenderer == null)
        {
            terrainMeshRenderer = GetComponent<TerrainMeshRenderer>();
            if (terrainMeshRenderer == null)
                terrainMeshRenderer = gameObject.AddComponent<TerrainMeshRenderer>();
        }
        
        if (meshSimplify == null)
        {
            currentGenerationCoroutine = null;
            yield break;
        }
        
        yield return StartCoroutine(meshSimplify.CreateShape(noiseScale, offset, heightMultiplier, scale));
        
        // get the generated mesh and update the renderer
        Mesh mesh = meshSimplify.GetMesh();
        if (mesh != null && terrainMeshRenderer != null)
            terrainMeshRenderer.UpdateMesh(mesh);
        
        // clear the coroutine reference 
        currentGenerationCoroutine = null;
    }
    
    // regenerate terrain
    [ContextMenu("Regenerate Terrain")]
    public void RegenerateTerrain()
    {
        // stop any existing generation coroutine
        if (currentGenerationCoroutine != null)
        {
            try
            {
                StopCoroutine(currentGenerationCoroutine);
            }
            catch (System.Exception)
            {
                // ignore errors 
            }
            currentGenerationCoroutine = null;
        }
        
        // Ensure components exist
        if (meshSimplify == null)
        {
            meshSimplify = GetComponent<MeshGen>();
            if (meshSimplify == null)
                meshSimplify = gameObject.AddComponent<MeshGen>();
        }
        
        if (terrainMeshRenderer == null)
        {
            terrainMeshRenderer = GetComponent<TerrainMeshRenderer>();
            if (terrainMeshRenderer == null)
                terrainMeshRenderer = gameObject.AddComponent<TerrainMeshRenderer>();
        }
        
        // validate size values
        if (width <= 0 || height <= 0)
        {
            width = 50;
            height = 50;
        }
        
        // sync size values before regenerating
        if (meshSimplify != null)
        {
            meshSimplify.SetSize(width, height);
        }
        
        // start new coroutine
        if (this != null && gameObject.activeInHierarchy)
        {
            currentGenerationCoroutine = StartCoroutine(GenerateTerrain());
        }
    }
    
    #region UISettings
    // public methods for UI settings
    public void SetWidth(int value) { width = value; }
    public void SetHeight(int value) { height = value; }
    public void SetScale(float value) { scale = value; }
    public void SetHeightMultiplier(float value) { heightMultiplier = value; }
    public void SetNoiseScale(float value) { noiseScale = value; }
    
    // getters for UI settings
    public int GetWidth() { return width; }
    public int GetHeight() { return height; }
    public float GetScale() { return scale; }
    public float GetHeightMultiplier() { return heightMultiplier; }
    public float GetNoiseScale() { return noiseScale; }
    
    #endregion
}
