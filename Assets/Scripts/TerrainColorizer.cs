using UnityEngine;

/// Applies height-based vertex coloring to terrain meshes.
public class TerrainColorizer : MonoBehaviour
{
    private const int COLOR_REGIONS = 4;
    private const float COLOR_REGION_SIZE = 1f / COLOR_REGIONS;  // 0.25 per region
    
    [Header("Terrain Colors")]
    [SerializeField] private Color waterColor = new Color(0.2f, 0.4f, 0.8f, 1f);      // blue
    [SerializeField] private Color grassColor = new Color(0.3f, 0.7f, 0.2f, 1f);     // green
    [SerializeField] private Color mountainColor = new Color(0.5f, 0.4f, 0.3f, 1f);  // brown
    [SerializeField] private Color peakColor = new Color(0.9f, 0.9f, 0.9f, 1f);      // white
    
    void Awake()
    {
        // register with GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.SetTerrainColorizer(this);
    }
    
    /// <summary>
    /// Calculates vertex color based on height using gradient blending between regions.
    /// </summary>
    /// <param name="height">Current vertex height</param>
    /// <param name="minHeight">Minimum height in the mesh</param>
    /// <param name="maxHeight">Maximum height in the mesh</param>
    /// <returns>Blended color based on height region</returns>
    public Color GetColorForHeight(float height, float minHeight, float maxHeight)
    {
        float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, height);
        
        float regionSize = COLOR_REGION_SIZE;
        
        if (normalizedHeight < regionSize)
        {
            // water - 0 to 0.25
            float t = normalizedHeight / regionSize;
            return Color.Lerp(waterColor, grassColor, t);
        }
        else if (normalizedHeight < regionSize * 2f)
        {
            // grass - 0.25 to 0.5
            float t = (normalizedHeight - regionSize) / regionSize;
            return Color.Lerp(grassColor, mountainColor, t);
        }
        else if (normalizedHeight < regionSize * 3f)
        {
            // mountain - 0.5 to 0.75
            float t = (normalizedHeight - regionSize * 2f) / regionSize;
            return Color.Lerp(mountainColor, peakColor, t);
        }
        else
        {
            // peak - 0.75 to 1.0
            return peakColor;
        }
    }
    
    // public getters for colors
    public Color GetWaterColor() { return waterColor; }
    public Color GetGrassColor() { return grassColor; }
    public Color GetMountainColor() { return mountainColor; }
    public Color GetPeakColor() { return peakColor; }
}

