using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class Brightness : MonoBehaviour
    {
        public VolumeProfile[] postProcessing = new VolumeProfile[4];
        public Slider brightnessSlider;
        [SerializeField] private TextMeshProUGUI sliderText;
        private ColorAdjustments[] colorAdj = new ColorAdjustments[4];
        private float originalBrightness;

        private void Start()
        {
            originalBrightness = Settings.Brightness;
            brightnessSlider.value = originalBrightness - 1;
            sliderText.text = (brightnessSlider.value + 1).ToString("0");

            for(int i = 0; i < postProcessing.Length; i++)
            {
                if (postProcessing[i].TryGet<ColorAdjustments>(out colorAdj[i]))
                {
                    colorAdj[i].postExposure.value = brightnessSlider.value + 1;
                    brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
                }
            }
            
        }
        private void OnBrightnessSliderChanged(float value)
        {
            foreach(ColorAdjustments child in colorAdj)
                child.postExposure.value = Mathf.Clamp(value, -4f, 2f) + 1;

            sliderText.text = (value + 1).ToString("0");
        }
        public void RestoreOriginalValues()
        {
            brightnessSlider.value = originalBrightness - 1;
            Settings.Brightness = brightnessSlider.value + 1;
        }
        public void ApplyChanges()
        {
            originalBrightness = brightnessSlider.value + 1;
            Settings.Brightness = originalBrightness;
        }
    }
}