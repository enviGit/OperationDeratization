using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener((v) => { sliderText.text = v.ToString("0"); }) ;
    }
}