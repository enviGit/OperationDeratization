using RatGamesStudios.OperationDeratization.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class Sensitivity : MonoBehaviour
    {
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private TextMeshProUGUI sliderText;
        [SerializeField] private PlayerShoot pointerSensitivity;
        private float originalSensitivity;

        private void Start()
        {
            originalSensitivity = Settings.Sensitivity;
            SetSensitivity(originalSensitivity);
            UpdateSliderText(sensitivitySlider.value);
            sensitivitySlider.onValueChanged.AddListener(UpdateSliderText);
        }
        public void SetSensitivity(float _value)
        {
            RefreshSlider(_value);
        }
        public void SetFromSensitivitySlider()
        {
            SetSensitivity(sensitivitySlider.value);
        }
        public void RefreshSlider(float _value)
        {
            sensitivitySlider.value = _value;
        }
        private void UpdateSliderText(float value)
        {
            int intValue = Mathf.RoundToInt(Mathf.Clamp(value * 10, 1, 20));
            float normalizedValue = Mathf.InverseLerp(1, 20, intValue);
            int displayedValue = Mathf.RoundToInt(normalizedValue * 10);
            sliderText.text = displayedValue.ToString();
        }
        public void RestoreOriginalValues()
        {
            SetSensitivity(originalSensitivity);
            Settings.Sensitivity = originalSensitivity;
        }
        public void ApplyChanges()
        {
            originalSensitivity = sensitivitySlider.value;
            Settings.Sensitivity = originalSensitivity;

            if(pointerSensitivity != null)
                pointerSensitivity.sensitivity = 3f * Settings.Sensitivity;
        }
    }
}