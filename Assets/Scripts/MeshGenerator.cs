using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField] private float scale = 10f;
    [SerializeField] private float heightMultiplier = 5f;
    
    [Header("Perlin Noise Settings")]
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private Vector2 offset = Vector2.zero;
    
    [Header("Spectral Synthesis (Octaves)")]
    [SerializeField] private bool useOctaves = true;
    [SerializeField] private int octaves = 4;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2f;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Start()
    {
        // Get or add required components
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        
        // Generate the terrain mesh
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        // Create new mesh
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Terrain";
        
        // Generate vertices
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        
        // Generate vertices with height from Perlin noise
        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float xCoord = (x / (float)width) * scale;
                float zCoord = (z / (float)height) * scale;
                
                // Calculate height using Perlin noise
                float y = CalculateHeight(xCoord, zCoord);
                
                int index = z * (width + 1) + x;
                vertices[index] = new Vector3(x - width / 2f, y, z - height / 2f);
                uvs[index] = new Vector2(x / (float)width, z / (float)height);
            }
        }
        
        // Generate triangles
        int[] triangles = new int[width * height * 6];
        int triIndex = 0;
        
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int bottomLeft = z * (width + 1) + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = (z + 1) * (width + 1) + x;
                int topRight = topLeft + 1;
                
                // First triangle (bottom-left, top-left, bottom-right)
                triangles[triIndex] = bottomLeft;
                triangles[triIndex + 1] = topLeft;
                triangles[triIndex + 2] = bottomRight;
                
                // Second triangle (bottom-right, top-left, top-right)
                triangles[triIndex + 3] = bottomRight;
                triangles[triIndex + 4] = topLeft;
                triangles[triIndex + 5] = topRight;
                
                triIndex += 6;
            }
        }
        
        // Assign data to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        
        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        // Assign mesh to components
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
    
    float CalculateHeight(float x, float z)
    {
        float heightValue = 0f;
        
        if (useOctaves)
        {
            // Spectral Synthesis: Multiple octaves of Perlin noise
            float amplitude = 1f;
            float frequency = noiseScale;
            float maxValue = 0f;
            
            for (int i = 0; i < octaves; i++)
            {
                float sampleX = (x + offset.x) * frequency;
                float sampleZ = (z + offset.y) * frequency;
                
                // Mathf.PerlinNoise returns value between 0 and 1
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ);
                
                // Add this octave's contribution
                heightValue += perlinValue * amplitude;
                
                // Track maximum possible value for normalization
                maxValue += amplitude;
                
                // Prepare for next octave
                amplitude *= persistence;  // Decrease amplitude
                frequency *= lacunarity;   // Increase frequency
            }
            
            // Normalize to 0-1 range
            heightValue /= maxValue;
        }
        else
        {
            // Single layer Perlin noise
            float sampleX = (x + offset.x) * noiseScale;
            float sampleZ = (z + offset.y) * noiseScale;
            heightValue = Mathf.PerlinNoise(sampleX, sampleZ);
        }
        
        // Scale by height multiplier
        return heightValue * heightMultiplier;
    }
    
    // Method to regenerate terrain (useful for testing different parameters)
    [ContextMenu("Regenerate Terrain")]
    public void RegenerateTerrain()
    {
        GenerateTerrain();
    }
}
