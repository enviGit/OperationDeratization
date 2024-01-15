using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resolutionText;
        [SerializeField] private Slider resolutionSlider;
        [SerializeField] private GameObject settings;
        private Resolution[] resolutions;
        private int currentResolutionIndex = 0;
        private int originalResolutionIndex = 0;
        private Fullscreen fullscreen;
        private SoundSettings soundSettings;
        private Brightness brightness;
        private Sensitivity sensitivity;
        private RunInBackground runInBg;
        private Quality quality;

        private void Start()
        {
            fullscreen = settings.GetComponent<Fullscreen>();
            soundSettings = settings.GetComponent<SoundSettings>();
            brightness = settings.GetComponent<Brightness>();
            sensitivity = settings.GetComponent<Sensitivity>();
            runInBg = settings.GetComponent<RunInBackground>();
            quality = settings.GetComponent<Quality>();
            resolutions = Screen.resolutions;
            originalResolutionIndex = Settings.ResolutionIndex;
            currentResolutionIndex = originalResolutionIndex;
            SetResolution(currentResolutionIndex);
            resolutionSlider.value = (float)currentResolutionIndex / (resolutions.Length - 1);
            resolutionSlider.onValueChanged.AddListener(OnResolutionSliderChanged);
            SetResolutionText(resolutions[currentResolutionIndex]);
        }
        private void SetResolutionText(Resolution resolution)
        {
            resolutionText.text = resolution.width + "x" + resolution.height;
        }
        private void SetResolution(int index)
        {
            currentResolutionIndex = index;
            SetResolutionText(resolutions[currentResolutionIndex]);
        }
        public void OnResolutionSliderChanged(float value)
        {
            int newIndex = Mathf.RoundToInt(value * (resolutions.Length - 1));

            if (newIndex != currentResolutionIndex)
                SetResolution(newIndex);
        }
        private void ApplyResolution(Resolution resolution)
        {
            SetResolutionText(resolution);
            Screen.SetResolution(resolution.width, resolution.height, Settings.FullScreen);
            originalResolutionIndex = currentResolutionIndex;
            Settings.ResolutionIndex = originalResolutionIndex;
        }
        private void SetAndApplyResolution(int newResolutionIndex)
        {
            currentResolutionIndex = newResolutionIndex;
            ApplyResolution(resolutions[currentResolutionIndex]);
        }
        private void RestoreOriginalValues()
        {
            SetAndApplyResolution(originalResolutionIndex);
            resolutionSlider.value = (float)originalResolutionIndex / (resolutions.Length - 1);
        }
        public void ApplyAllChanges()
        {
            SetAndApplyResolution(currentResolutionIndex);

            if (fullscreen != null)
                fullscreen.ApplyChanges();
            if (quality != null)
                quality.ApplyChanges();

            soundSettings.ApplyChanges();
            brightness.ApplyChanges();
            sensitivity.ApplyChanges();
            PlayerPrefs.Save();
        }
        public void AbortChanges()
        {
            RestoreOriginalValues();

            if (fullscreen != null)
                fullscreen.RestoreOriginalState();
            if (runInBg != null)
                runInBg.RestoreOriginalState();
            if (quality != null)
                quality.RestoreOriginalValues();

            soundSettings.RestoreOriginalValues();
            brightness.RestoreOriginalValues();
            sensitivity.RestoreOriginalValues();
            PlayerPrefs.Save();
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}