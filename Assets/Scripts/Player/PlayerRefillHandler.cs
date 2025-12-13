using UnityEngine;
using TMPro;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerRefillHandler : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject loadingSliderObject;
        public Material sliderMaterial;
        public TextMeshProUGUI sliderValueText;

        public void SetSliderVisible(bool visible)
        {
            if (loadingSliderObject) loadingSliderObject.SetActive(visible);
        }

        public void UpdateSlider(float progress, float maxTime)
        {
            if (sliderMaterial)
            {
                float segmentCount = sliderMaterial.GetFloat("_SegmentCount");
                sliderMaterial.SetFloat("_RemovedSegments", Mathf.Lerp(segmentCount, 0, progress));
            }
            if (sliderValueText) sliderValueText.text = $"{progress * maxTime:F1}s";
        }
    }
}