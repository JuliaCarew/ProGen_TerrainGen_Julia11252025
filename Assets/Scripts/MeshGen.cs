using System.Collections;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    [Header("Mesh Settings")]
    [SerializeField] private int xSize = 50;
    [SerializeField] private int zSize = 50;

    public void SetSize(int x, int z)
    {
        xSize = x;
        zSize = z;
    }

    private Vector3[] vertices;
    private int[] triangles;

    public IEnumerator CreateShape(float noiseScale = 0.1f, Vector2 offset = default, float heightMultiplier = 5f, float scale = 10f)
    {
        // create vertices array
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        
        // generate vertices
        int i = 0;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // world position 
                float worldX = (float)x;
                float worldZ = (float)z;
                
                // perlin noise 
                float normalizedX = (float)x / (float)xSize;
                float normalizedZ = (float)z / (float)zSize;
                float sampleX = (normalizedX * scale * noiseScale) + offset.x;
                float sampleZ = (normalizedZ * scale * noiseScale) + offset.y;
                float y = Mathf.PerlinNoise(sampleX, sampleZ) * heightMultiplier;
                
                // track min/max 
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
                
                // vertex positions
                vertices[i] = new Vector3(worldX, y, worldZ);
                i++;
            }
        }
        
        Debug.Log($"MeshGen: Perlin noise Y range: {minY} to {maxY} (heightMultiplier: {heightMultiplier}, noiseScale: {noiseScale}, scale: {scale})");
        Debug.Log($"MeshGen: Vertices generated. Starting triangle generation...");
        
        // create triangles array
        triangles = new int[xSize * zSize * 6];
        
        int vert = 0;
        int tris = 0;
        int totalTriangles = xSize * zSize;
        int currentTriangle = 0;
        
        // generate triangles
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
                currentTriangle++;

            }
            vert++;
        }
        
        Debug.Log($"MeshGen: Triangle generation completed. Total triangles: {totalTriangles}");
        
        yield return null;
    }

    public Mesh GetMesh()
    {
        if (vertices == null || triangles == null) return null;
        
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Mesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }
}