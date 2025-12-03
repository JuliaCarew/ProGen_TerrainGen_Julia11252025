using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// UI Manager for terrain generation controls.
public class TerrainUI : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private MeshGenerator meshGenerator;
    
    [Header("Terrain Settings Sliders")]
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Slider heightMultiplierSlider;
    
    [Header("Noise Settings Sliders")]
    [SerializeField] private Slider noiseScaleSlider;
    
    [Header("UI Labels")]
    [SerializeField] private TextMeshProUGUI scaleLabel;
    [SerializeField] private TextMeshProUGUI heightMultiplierLabel;
    [SerializeField] private TextMeshProUGUI noiseScaleLabel;
    
    #endregion

    void Start()
    {
        // find MeshGenerator 
        if (meshGenerator == null)
            meshGenerator = FindObjectOfType<MeshGenerator>();
        
        SubscribeToSliders();
    }

    #region Slider Event Handlers

    void OnScaleChanged(float value)
    {
        if (meshGenerator != null)
        {
            meshGenerator.SetScale(value);
            meshGenerator.RegenerateTerrain();
        }
        UpdateLabel(scaleLabel, "Scale: ", value);
    }

    void OnHeightMultiplierChanged(float value)
    {
        if (meshGenerator != null)
        {
            meshGenerator.SetHeightMultiplier(value);
            meshGenerator.RegenerateTerrain();
        }
        UpdateLabel(heightMultiplierLabel, "Height Mult: ", value);
    }

    void OnNoiseScaleChanged(float value)
    {
        if (meshGenerator != null)
        {
            meshGenerator.SetNoiseScale(value);
            meshGenerator.RegenerateTerrain();
        }
        UpdateLabel(noiseScaleLabel, "Noise Scale: ", value);
    }

    #endregion

    #region Helper Methods

    /// updates a UI label with a formatted value.
    void UpdateLabel(TextMeshProUGUI label, string prefix, float value)
    {
        if (label != null)
            label.text = prefix + value.ToString("F2");
    }
    
    void SubscribeToSliders()
    {
        if (scaleSlider != null)
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        
        if (heightMultiplierSlider != null)
            heightMultiplierSlider.onValueChanged.AddListener(OnHeightMultiplierChanged);
        
        if (noiseScaleSlider != null)
            noiseScaleSlider.onValueChanged.AddListener(OnNoiseScaleChanged);
    }

    #endregion
}