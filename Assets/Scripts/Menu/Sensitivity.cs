using UnityEngine;
using UnityEngine.UI;

public class Sensitivity : MonoBehaviour
{
    public Slider sensitivitySlider;

    void Start()
    {
        sensitivitySlider.value = 0.5f;
    }

    void Update()
    {
        float sensitivityMultiplier = sensitivitySlider.value;
        //PlayerPrefs.SetFloat("SensitivityMultiplier", sensitivitySlider.value);
        //PlayerPrefs.Save();
    }
}