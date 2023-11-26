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

        // Ustawienie wartości początkowej Slidera na 50% (0.5)
        brightnessSlider.value = 0f;

        // Inicjalizacja slidera na podstawie początkowej wartości postExposure
        colorAdj.postExposure.value = brightnessSlider.value;

        // Dodanie metody obsługującej zmiany wartości Slidera
        brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
    }

    // Metoda obsługująca zmiany wartości Slidera
    private void OnBrightnessSliderChanged(float value)
    {
        if (colorAdj != null)
        {
            // Ograniczenie wartości postExposure do zakresu od -3 do 3
            colorAdj.postExposure.value = Mathf.Clamp(value, -3f, 3f);
        }
    }
}
