using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class Brightness : MonoBehaviour
    {
        public VolumeProfile postProcessing;
        public Slider brightnessSlider;
        private ColorAdjustments colorAdj;
        private Vignette vignette;
        private TextMeshProUGUI sliderText;
        private float originalBrightness;

        private void Start()
        {
            sliderText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            originalBrightness = Settings.Brightness;
            brightnessSlider.value = originalBrightness;
            sliderText.text = (brightnessSlider.value + 1).ToString("0");

            if (postProcessing.TryGet<ColorAdjustments>(out colorAdj))
            {
                colorAdj.postExposure.value = brightnessSlider.value;
                brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
            }
            if (postProcessing.TryGet<Vignette>(out vignette))
                vignette.intensity.value = 0f;
        }
        private void OnBrightnessSliderChanged(float value)
        {
            colorAdj.postExposure.value = Mathf.Clamp(value, -4f, 2f);
            sliderText.text = (value + 1).ToString("0");
        }
        public void RestoreOriginalValues()
        {
            brightnessSlider.value = originalBrightness;
            Settings.Brightness = brightnessSlider.value;
        }
        public void ApplyChanges()
        {
            originalBrightness = brightnessSlider.value;
            Settings.Brightness = originalBrightness;
        }
    }
}