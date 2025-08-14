using UnityEngine;
using UnityEngine.UI;
using EasyPeasyFirstPersonController;
using TMPro; // --- ADD THIS LINE ---
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public Slider sensitivitySlider;
    public TMP_Dropdown graphicsDropdown; // --- CHANGE THIS LINE ---
    
    private const string MouseSensitivityKey = "MouseSensitivity";
    private const string GraphicsQualityKey = "GraphicsQuality";

    void Start()
    {
        LoadSettings();
        
        // Listen for changes from the UI
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }
        
        if (graphicsDropdown != null)
        {
            // --- UPDATED Listener for TMPro Dropdown ---
            graphicsDropdown.onValueChanged.AddListener(UpdateGraphics);
        }
    }

    void OnEnable()
    {
        // When the panel is enabled, make sure the UI reflects the current settings
        LoadSettings();
    }

    public void LoadSettings()
    {
        // --- Load and apply mouse sensitivity ---
        float loadedSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 25f); // 25f is default
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = loadedSensitivity;
        }
        UpdateSensitivity(loadedSensitivity);

        // --- Load and apply graphics quality ---
        int loadedQuality = PlayerPrefs.GetInt(GraphicsQualityKey, 2); // 2 is default (High)
        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = loadedQuality;
        }
        UpdateGraphics(loadedQuality);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(MouseSensitivityKey, sensitivitySlider.value);
        PlayerPrefs.SetInt(GraphicsQualityKey, graphicsDropdown.value);
        PlayerPrefs.Save();
        Debug.Log("Settings saved!");
    }

    private void UpdateSensitivity(float newSensitivity)
    {
        // Find the FirstPersonController and update its sensitivity
        FirstPersonController controller = FindObjectOfType<FirstPersonController>();
        if (controller != null)
        {
            controller.mouseSensitivity = newSensitivity;
        }
    }

    private void UpdateGraphics(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}