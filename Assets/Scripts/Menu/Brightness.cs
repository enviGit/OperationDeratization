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
        if (!postProcessing.TryGet<ColorAdjustments>(out colorAdj))
            return;

        brightnessSlider.value = 0f;
        colorAdj.postExposure.value = brightnessSlider.value;
        brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
    }
    private void OnBrightnessSliderChanged(float value)
    {
        if (colorAdj != null)
            colorAdj.postExposure.value = Mathf.Clamp(value, -4f, 2f);
    }
}
