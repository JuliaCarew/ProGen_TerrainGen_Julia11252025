using System.Collections;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    #region Constants and Variables
    private const int DEFAULT_MESH_SIZE = 50;
    private const int TRIANGLES_PER_QUAD = 2;
    private const int INDICES_PER_TRIANGLE = 3;
    private const int TOTAL_INDICES_PER_QUAD = TRIANGLES_PER_QUAD * INDICES_PER_TRIANGLE; // 6

    [Header("Mesh Settings")]
    [SerializeField] private int xSize = DEFAULT_MESH_SIZE;
    [SerializeField] private int zSize = DEFAULT_MESH_SIZE;

    public void SetSize(int x, int z)
    {
        xSize = x;
        zSize = z;
    }

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    private float minHeight;
    private float maxHeight;
    
    #endregion
    
    void Awake()
    {
        // register with GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.SetMeshGen(this);
    }

    /// <summary>
    /// Generates a procedural terrain mesh using Perlin noise for height generation.
    /// </summary>
    /// <param name="noiseScale">Controls the overall frequency of the Perlin noise</param>
    /// <param name="offset">Offsets the Perlin noise sampling position</param>
    /// <param name="heightMultiplier">Multiplies the Perlin noise value to control terrain height</param>
    /// <param name="scale">Additional frequency multiplier</param>
    /// <param name="colorizer">component to apply height-based vertex colors</param>
    public IEnumerator CreateShape(float noiseScale = 0.1f, Vector2 offset = default, float heightMultiplier = 5f, float scale = 10f, TerrainColorizer colorizer = null)
    {
        // calculate total vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        colors = new Color[vertices.Length];
        
        // generate vertices with Perlin noise height values
        int i = 0;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        
        // iterate through grid positions
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // world position
                float worldX = (float)x;
                float worldZ = (float)z;
                
                // normalized position
                float normalizedX = (float)x / (float)xSize;
                float normalizedZ = (float)z / (float)zSize;
                
                // apply scale factors to control noise frequency
                float sampleX = (normalizedX * scale * noiseScale) + offset.x;
                float sampleZ = (normalizedZ * scale * noiseScale) + offset.y;
                
                // sample Perlin noise (returns 0-1 range) and multiply by heightMultiplier
                float y = Mathf.PerlinNoise(sampleX, sampleZ) * heightMultiplier;
                
                // track min/max heights for color 
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
                
                // store vertex position
                vertices[i] = new Vector3(worldX, y, worldZ);
                i++;
            }
        }
        
        // store min/max for color
        minHeight = minY;
        maxHeight = maxY;
        
        // get colors based on height
        if (colorizer != null)
        {
            // handle edge case where all vertices are at same height
            if (Mathf.Approximately(minHeight, maxHeight))
            {
                Color defaultColor = colorizer.GetColorForHeight(minHeight, minHeight, maxHeight + 0.001f);
                for (i = 0; i < colors.Length; i++)
                    colors[i] = defaultColor;
            }
            else
            {
                for (i = 0; i < vertices.Length; i++)
                    colors[i] = colorizer.GetColorForHeight(vertices[i].y, minHeight, maxHeight);
            }
        }
        
        // TRIANGLE GENERATION:
        triangles = new int[xSize * zSize * TOTAL_INDICES_PER_QUAD];
        
        int vert = 0;  // current vertex index
        int tris = 0;  // current triangle index
        
        // generate triangles for each quad in the grid
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // TRIANGLE 1: Bottom-left, top-left, bottom-right
                triangles[tris + 0] = vert + 0; 
                triangles[tris + 1] = vert + xSize + 1; 
                triangles[tris + 2] = vert + 1; 
                
                // TRIANGLE 2: Bottom-right, top-left, top-right
                triangles[tris + 3] = vert + 1;                    
                triangles[tris + 4] = vert + xSize + 1;          
                triangles[tris + 5] = vert + xSize + 2;           

                vert++;     
                tris += TOTAL_INDICES_PER_QUAD; 
            }
            vert++;  // skip to next row 
        }
        
        yield return null;
    }

    public Mesh GetMesh()
    {
        if (vertices == null || triangles == null) return null;
        
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Mesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        // ensure colors array exists and has correct length
        if (colors == null || colors.Length != vertices.Length)
        {
            colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
        }
        
        mesh.colors = colors;
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }
}