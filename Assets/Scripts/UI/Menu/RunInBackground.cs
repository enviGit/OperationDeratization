using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class RunInBackground : MonoBehaviour
    {
        private bool originalValue;
        [SerializeField] private Toggle toggle;

        private void Awake()
        {
            originalValue = Settings.RunInBg;
            Application.runInBackground = originalValue;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            toggle.isOn = originalValue;
        }
        private void OnToggleValueChanged(bool newValue)
        {
            Settings.RunInBg = newValue;
        }
        public void Change()
        {
            Application.runInBackground = !Application.runInBackground;
        }
        public void RestoreOriginalState()
        {
            toggle.isOn = originalValue;
            Settings.RunInBg = toggle.isOn;
        }
        public void ApplyChanges()
        {
            originalValue = toggle.isOn;
            Settings.RunInBg = originalValue;
        }
    }
}