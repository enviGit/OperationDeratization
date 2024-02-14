using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class VSyncControl : MonoBehaviour
    {
        private int originalValue;
        [SerializeField] private Toggle toggle;

        private void Awake()
        {
            originalValue = Settings.VSync ? 1 : 0;
            QualitySettings.vSyncCount = originalValue;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            toggle.isOn = originalValue != 0;
        }
        private void OnToggleValueChanged(bool newValue)
        {
            Settings.VSync = newValue;
            QualitySettings.vSyncCount = newValue ? 1 : 0;
        }
        public void Change()
        {
            Settings.VSync = !Settings.VSync;
            QualitySettings.vSyncCount = Settings.VSync ? 1 : 0;
        }
        public void RestoreOriginalState()
        {
            toggle.isOn = originalValue != 0;
            Settings.VSync = toggle.isOn;
            QualitySettings.vSyncCount = originalValue;
        }
        public void ApplyChanges()
        {
            originalValue = toggle.isOn ? 1 : 0;
            Settings.VSync = toggle.isOn;
            QualitySettings.vSyncCount = originalValue;
        }
    }
}