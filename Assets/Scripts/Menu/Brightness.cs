using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Brightness : MonoBehaviour
{
    public VolumeProfile postProcessing;
    private Slider brightnessSlider;
    private ColorAdjustments colorAdj;
    private Vignette vignette;

    private void Start()
    {
        brightnessSlider = GetComponent<Slider>();
        SetBrightness(Settings.Brightness);

        if (postProcessing.TryGet<Vignette>(out vignette))
            vignette.intensity.value = 0f;
        if (postProcessing.TryGet<ColorAdjustments>(out colorAdj))
            SetBrightness(Settings.Brightness);
    }
    public void SetBrightness(float _value)
    {
        RefreshSlider(_value);
        Settings.Brightness = _value;
        //colorAdj.postExposure.value = _value;
    }
    public void SetFromBrightnessSlider()
    {
        SetBrightness(brightnessSlider.value);
    }
    public void RefreshSlider(float _value)
    {
        brightnessSlider.value = _value;
    }
}
