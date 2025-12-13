using RatGamesStudios.OperationDeratization.Interactables;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerRefillHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInteract interact;
        [SerializeField] private PlayerInventory inventory;

        [Header("UI References")]
        [SerializeField] private GameObject loadingSliderObject;
        [SerializeField] private Material sliderMaterial;
        [SerializeField] private TextMeshProUGUI sliderValueText;
        [SerializeField] private TextMeshProUGUI warningText;

        private StringBuilder warningBuilder = new StringBuilder();
        private Coroutine hideWarningCoroutine;

        private void Start()
        {
            if (loadingSliderObject) loadingSliderObject.SetActive(false);
            if (warningText) warningText.text = "";

            if (interact == null) interact = GetComponent<PlayerInteract>();
            if (inventory == null) inventory = GetComponent<PlayerInventory>();
        }

        public IEnumerator ProcessRefill(AmmoBox ammoBox)
        {
            if (ammoBox == null) yield break;

            ammoBox.isFilling = true;
            if (loadingSliderObject) loadingSliderObject.SetActive(true);

            float startTime = Time.time;
            float duration = 2.0f;

            while (Time.time - startTime < duration)
            {
                if (interact.CurrentAmmoBox != ammoBox)
                {
                    CancelRefill(ammoBox);
                    yield break;
                }

                float progress = (Time.time - startTime) / duration;
                UpdateSliderVisuals(progress);

                yield return null;
            }

            RefillWeapons();

            CancelRefill(ammoBox);
        }

        private void CancelRefill(AmmoBox ammoBox)
        {
            if (ammoBox) ammoBox.isFilling = false;
            if (loadingSliderObject) loadingSliderObject.SetActive(false);
        }

        private void UpdateSliderVisuals(float progress)
        {
            if (sliderMaterial)
            {
                float segmentCount = sliderMaterial.GetFloat("_SegmentCount");
                sliderMaterial.SetFloat("_RemovedSegments", Mathf.Lerp(segmentCount, 0, progress));
            }
            if (sliderValueText) sliderValueText.text = $"{progress * 2.0f:F1}s";
        }

        private void RefillWeapons()
        {
            if (inventory == null) return;

            foreach (Gun gun in inventory.weapons)
            {
                if (gun != null && (gun.gunStyle == GunStyle.Primary || gun.gunStyle == GunStyle.Secondary))
                {
                    if (gun.maxAmmoCount < gun.magazineSize * 3)
                    {
                        gun.maxAmmoCount += gun.magazineSize;
                    }
                    else
                    {
                        ShowWarning($"Cannot carry more {gun.gunName} ammo!");
                    }
                }
            }
        }

        public void ShowWarning(string message)
        {
            if (warningText == null) return;

            warningText.text = message;
            warningText.color = Color.white;

            if (hideWarningCoroutine != null) StopCoroutine(hideWarningCoroutine);
            hideWarningCoroutine = StartCoroutine(FadeWarning());
        }

        private IEnumerator FadeWarning()
        {
            yield return new WaitForSeconds(2f);

            float fadeDuration = 1.5f;
            float startFade = Time.time;
            Color color = warningText.color;

            while (Time.time < startFade + fadeDuration)
            {
                float t = (Time.time - startFade) / fadeDuration;
                color.a = Mathf.Lerp(1f, 0f, t);
                warningText.color = color;
                yield return null;
            }

            warningText.text = "";
        }
    }
}