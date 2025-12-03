using System.Collections;
using UnityEngine;

/// Main manager class for terrain generation system.
/// Coordinates mesh generation, rendering, and coloring components.
public class MeshGenerator : MonoBehaviour
{
    #region Variables

    private const int DEFAULT_TERRAIN_SIZE = 50;
    private const float DEFAULT_SCALE = 5f;
    private const float DEFAULT_HEIGHT_MULTIPLIER = 10f;
    private const float DEFAULT_NOISE_SCALE = 0.3f;
    private const int MIN_TERRAIN_SIZE = 1;

    [Header("Terrain Settings")]
    [SerializeField] private int width = DEFAULT_TERRAIN_SIZE;
    [SerializeField] private int height = DEFAULT_TERRAIN_SIZE;
    [SerializeField] private float scale = DEFAULT_SCALE;
    [SerializeField] private float heightMultiplier = DEFAULT_HEIGHT_MULTIPLIER;
    
    [Header("Perlin Noise Settings")]
    [SerializeField] private float noiseScale = DEFAULT_NOISE_SCALE;
    [SerializeField] private Vector2 offset = Vector2.zero;
    
    private MeshGen meshSimplify;
    private TerrainMeshRenderer terrainMeshRenderer;
    private TerrainColorizer terrainColorizer;
    private Coroutine currentGenerationCoroutine;

    #endregion
    
    #region Helper Methods
    
    /// ensure all required components are initialized via GameManager
    private void EnsureComponents()
    {
        if (meshSimplify == null)
        {
            meshSimplify = GameManager.Instance.MeshGen;
            if (meshSimplify == null)
            {
                meshSimplify = gameObject.AddComponent<MeshGen>();
                GameManager.Instance.SetMeshGen(meshSimplify);
            }
        }
        
        if (terrainMeshRenderer == null)
        {
            terrainMeshRenderer = GameManager.Instance.TerrainMeshRenderer;
            if (terrainMeshRenderer == null)
            {
                terrainMeshRenderer = gameObject.AddComponent<TerrainMeshRenderer>();
                GameManager.Instance.SetTerrainMeshRenderer(terrainMeshRenderer);
            }
        }
        
        if (terrainColorizer == null)
        {
            terrainColorizer = GameManager.Instance.TerrainColorizer;
            if (terrainColorizer == null)
            {
                terrainColorizer = gameObject.AddComponent<TerrainColorizer>();
                GameManager.Instance.SetTerrainColorizer(terrainColorizer);
            }
        }
    }
    
    #endregion
    
    void Start()
    {
        EnsureComponents();
        
        // set mesh size from terrain settings
        if (meshSimplify != null)
            meshSimplify.SetSize(width, height);
        
        // generate terrain mesh 
        StartCoroutine(GenerateTerrain());
    }
    
    void Awake()
    {
        // register with GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.SetMeshGenerator(this);
        
        EnsureComponents();
    }

    IEnumerator GenerateTerrain()
    {
        EnsureComponents();
        
        if (meshSimplify == null)
        {
            currentGenerationCoroutine = null;
            yield break;
        }
        
        yield return StartCoroutine(meshSimplify.CreateShape(noiseScale, offset, heightMultiplier, scale, terrainColorizer));
        
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
            catch (System.Exception) {}
            currentGenerationCoroutine = null;
        }
        
        EnsureComponents();
        
        // validate size values
        if (width < MIN_TERRAIN_SIZE || height < MIN_TERRAIN_SIZE)
        {
            width = DEFAULT_TERRAIN_SIZE;
            height = DEFAULT_TERRAIN_SIZE;
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
    public void SetScale(float value) { scale = value; }
    public void SetHeightMultiplier(float value) { heightMultiplier = value; }
    public void SetNoiseScale(float value) { noiseScale = value; }
    
    #endregion
}
