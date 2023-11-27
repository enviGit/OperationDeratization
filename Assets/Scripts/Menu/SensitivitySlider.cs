using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;

    private void Start()
    {
        slider = GetComponent<Slider>();
        UpdateSliderText(slider.value);
        slider.onValueChanged.AddListener(UpdateSliderText);
    }

    private void UpdateSliderText(float value)
    {
        int intValue = Mathf.RoundToInt(Mathf.Clamp(value * 10, 1, 30));
        sliderText.text = intValue.ToString();
    }
}
