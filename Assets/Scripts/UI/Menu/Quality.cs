using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace RatGamesStudios.OperationDeratization
{
    public class Quality : MonoBehaviour
    {
        [SerializeField] private RenderPipelineAsset[] qualityLevels;
        [SerializeField] TMP_Dropdown dropdown;
        private int originalQuality;

        private void Start()
        {
            originalQuality = Settings.QualityPreset;
            dropdown.value = originalQuality;
        }
        public void ChangeLevel(int value)
        {
            QualitySettings.SetQualityLevel(value, true);
            QualitySettings.renderPipeline = qualityLevels[value];
        }
        public void RestoreOriginalValues()
        {
            dropdown.value = originalQuality;
            Settings.QualityPreset = dropdown.value;
        }
        public void ApplyChanges()
        {
            originalQuality = dropdown.value;
            Settings.QualityPreset = originalQuality;
        }
    }
}