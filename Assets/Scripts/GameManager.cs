using UnityEngine;

/// Global manager that provides centralized access to all terrain generation scripts
public class GameManager : MonoBehaviour
{
    #region Singleton
    
    private static GameManager _instance;
    
    /// Global instance of GameManager
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    #endregion
    
    #region Script References
    
    [Header("Terrain Generation Scripts")]
    [SerializeField] private MeshGenerator meshGenerator;
    [SerializeField] private MeshGen meshGen;
    [SerializeField] private TerrainMeshRenderer terrainMeshRenderer;
    [SerializeField] private TerrainColorizer terrainColorizer;
    
    #endregion
    
    #region Public Properties
    
    public MeshGenerator MeshGenerator
    {
        get
        {
            if (meshGenerator == null)
                meshGenerator = FindObjectOfType<MeshGenerator>();
            return meshGenerator;
        }
    }
    
    public MeshGen MeshGen
    {
        get
        {
            if (meshGen == null)
                meshGen = FindObjectOfType<MeshGen>();
            return meshGen;
        }
    }
    
    public TerrainMeshRenderer TerrainMeshRenderer
    {
        get
        {
            if (terrainMeshRenderer == null)
                terrainMeshRenderer = FindObjectOfType<TerrainMeshRenderer>();
            return terrainMeshRenderer;
        }
    }
    
    public TerrainColorizer TerrainColorizer
    {
        get
        {
            if (terrainColorizer == null)
                terrainColorizer = FindObjectOfType<TerrainColorizer>();
            return terrainColorizer;
        }
    }
    
    #endregion
    
    #region Unity Lifecycle
    
    void Awake()
    {
        // singleton 
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeReferences();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // auto-find references if not assigned in inspector
        InitializeReferences();
    }
    
    #endregion
    
    #region Private Methods
    
    /// initializes script references by finding them in the scene if not already assigned.
    private void InitializeReferences()
    {
        if (meshGenerator == null)
            meshGenerator = FindObjectOfType<MeshGenerator>();
        
        if (meshGen == null)
            meshGen = FindObjectOfType<MeshGen>();
        
        if (terrainMeshRenderer == null)
            terrainMeshRenderer = FindObjectOfType<TerrainMeshRenderer>();
        
        if (terrainColorizer == null)
            terrainColorizer = FindObjectOfType<TerrainColorizer>();
    }
    
    /// manually set script references 
    public void SetMeshGenerator(MeshGenerator mg) => meshGenerator = mg;
    public void SetMeshGen(MeshGen mg) => meshGen = mg;
    public void SetTerrainMeshRenderer(TerrainMeshRenderer tmr) => terrainMeshRenderer = tmr;
    public void SetTerrainColorizer(TerrainColorizer tc) => terrainColorizer = tc;
    
    #endregion
}

