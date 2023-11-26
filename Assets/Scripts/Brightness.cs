using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Brightness : MonoBehaviour
{
    public VolumeProfile postProcessing;
    public Slider brightnessSlider;

    private ColorAdjustments colorAdj;

    private void Start()
    {
        if (postProcessing == null)
        {
            Debug.LogError("VolumeProfile not assigned!");
            return;
        }

        if (!postProcessing.TryGet<ColorAdjustments>(out colorAdj))
        {
            Debug.LogError("ColorAdjustments not found in VolumeProfile!");
            return;
        }

        if (brightnessSlider == null)
        {
            Debug.LogError("Slider not assigned!");
            return;
        }

        // Ustawienie wartoœci pocz¹tkowej Slidera na 50% (0.5)
        brightnessSlider.value = 0f;

        // Inicjalizacja slidera na podstawie pocz¹tkowej wartoœci postExposure
        colorAdj.postExposure.value = brightnessSlider.value;

        // Dodanie metody obs³uguj¹cej zmiany wartoœci Slidera
        brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
    }

    // Metoda obs³uguj¹ca zmiany wartoœci Slidera
    private void OnBrightnessSliderChanged(float value)
    {
        if (colorAdj != null)
        {
            // Ograniczenie wartoœci postExposure do zakresu od -3 do 3
            colorAdj.postExposure.value = Mathf.Clamp(value, -3f, 3f);
        }
    }
}
