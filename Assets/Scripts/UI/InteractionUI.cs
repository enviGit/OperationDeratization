using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI
{
    public class InteractionUI : MonoBehaviour
    {
        [Header("General Prompt")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private GameObject promptPanel;

        [Header("Weapon Comparison")]
        [SerializeField] private GameObject comparisonPanel;
        [SerializeField] private Image singlePickupIcon;
        [SerializeField] private Image currentWeaponIcon;
        [SerializeField] private Image newWeaponIcon;
        [SerializeField] private TextMeshProUGUI swapText;

        private void Start()
        {
            HideAll();
        }

        public void SetPrompt(string text, Sprite icon = null)
        {
            if (comparisonPanel) comparisonPanel.SetActive(false);

            if (string.IsNullOrEmpty(text))
            {
                if (promptText) promptText.text = "";
                if (promptPanel) promptPanel.SetActive(false);
                if (singlePickupIcon) singlePickupIcon.gameObject.SetActive(false);
            }
            else
            {
                if (promptPanel) promptPanel.SetActive(true);
                if (promptText) promptText.text = text;

                if (singlePickupIcon)
                {
                    if (icon != null)
                    {
                        singlePickupIcon.gameObject.SetActive(true);
                        singlePickupIcon.sprite = icon;
                        singlePickupIcon.preserveAspect = true;
                    }
                    else
                    {
                        singlePickupIcon.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void ShowWeaponComparison(Gun newGun, Gun currentGun)
        {
            if (promptPanel) promptPanel.SetActive(false);
            if (comparisonPanel) comparisonPanel.SetActive(true);

            if (swapText) swapText.text = $"Swap for {newGun.gunName}";

            if (currentWeaponIcon)
            {
                if (currentGun != null)
                {
                    currentWeaponIcon.gameObject.SetActive(true);
                    currentWeaponIcon.sprite = currentGun.activeGunIcon;
                }
                else
                {
                    currentWeaponIcon.gameObject.SetActive(false);
                }
            }

            if (newWeaponIcon)
            {
                newWeaponIcon.gameObject.SetActive(true);
                newWeaponIcon.sprite = newGun.activeGunIcon;
            }
        }

        public void HideAll()
        {
            if (promptPanel) promptPanel.SetActive(false);
            if (comparisonPanel) comparisonPanel.SetActive(false);
            if (promptText) promptText.text = "";
            if (singlePickupIcon) singlePickupIcon.gameObject.SetActive(false);
        }
    }
}