using UnityEngine;
using UnityEngine.UI;

public class Fullscreen : MonoBehaviour
{
    private bool originalFullScreen;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        originalFullScreen = Settings.FullScreen;
        Screen.fullScreen = originalFullScreen;
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        toggle.isOn = originalFullScreen;
    }
    private void OnToggleValueChanged(bool newValue)
    {
        Settings.FullScreen = newValue;
    }
    public void Change()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void RestoreOriginalState()
    {
        Screen.fullScreen = originalFullScreen;
        toggle.isOn = originalFullScreen;
        Settings.FullScreen = originalFullScreen;
    }
    public void ApplyChanges()
    {
        originalFullScreen = Screen.fullScreen;
        toggle.isOn = originalFullScreen;
        Settings.FullScreen = originalFullScreen;
    }
}