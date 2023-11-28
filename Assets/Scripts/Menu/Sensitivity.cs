using UnityEngine;
using UnityEngine.UI;

public class Sensitivity : MonoBehaviour
{
    Slider sensitivitySlider;

    private void Start()
    {
        sensitivitySlider = GetComponent<Slider>();
        SetSensitivity(Settings.Sensitivity);
    }
    public void SetSensitivity(float _value)
    {
        RefreshSlider(_value);
        Settings.Sensitivity = _value;
    }
    public void SetVolumeFromSensitivitySlider()
    {
        SetSensitivity(sensitivitySlider.value);
    }
    public void RefreshSlider(float _value)
    {
        sensitivitySlider.value = _value;
    }
}