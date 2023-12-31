using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
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
            toggle.isOn = originalFullScreen;
            Settings.FullScreen = toggle.isOn;
        }
        public void ApplyChanges()
        {
            originalFullScreen = toggle.isOn;
            Settings.FullScreen = originalFullScreen;
        }
    }
}