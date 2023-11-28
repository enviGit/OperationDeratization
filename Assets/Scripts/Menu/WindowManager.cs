using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowManager : MonoBehaviour
{
    private const string RESOLUTION_PREF_KEY = "resolution";
    [SerializeField] private TextMeshProUGUI resolutionText;
    [SerializeField] private Slider resolutionSlider;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    private void Start()
    {
        resolutions = Screen.resolutions;
        currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, 0);
        SetResolution(currentResolutionIndex);
    }
    private void SetResolutionText(Resolution resolution)
    {
        resolutionText.text = resolution.width + "x" + resolution.height;
    }
    private void SetResolution(int index)
    {
        currentResolutionIndex = index;
        SetResolutionText(resolutions[currentResolutionIndex]);
        resolutionSlider.value = (float)currentResolutionIndex / (resolutions.Length - 1);
    }
    public void OnResolutionSliderChanged()
    {
        int newIndex = Mathf.RoundToInt(resolutionSlider.value * (resolutions.Length - 1));

        if (newIndex != currentResolutionIndex)
            SetResolution(newIndex);
    }
    private void ApplyResolution(Resolution resolution)
    {
        SetResolutionText(resolution);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, currentResolutionIndex);
    }
    private void ApplyCurrentResolution()
    {
        ApplyResolution(resolutions[currentResolutionIndex]);
    }
    private void SetAndApplyResolution(int newResolutionIndex)
    {
        currentResolutionIndex = newResolutionIndex;
        ApplyCurrentResolution();
    }
    public void ApplyChanges()
    {
        SetAndApplyResolution(currentResolutionIndex);
    }
}