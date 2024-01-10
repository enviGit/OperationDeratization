using RatGamesStudios.OperationDeratization.Interactables;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("Reference")]
        private Gun currentWeapon;
        private PlayerInventory inventory;
        private PlayerInteract interact;
        [HideInInspector] public AmmoBox ammoRefill;

        [Header("Player UI")]
        public TextMeshProUGUI promptText;
        public TextMeshProUGUI markText;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI ammoRefillPrompt;
        private float fadeDuration = 1.5f;
        private Coroutine hideCoroutine;
        private const int maxLines = 2;

        [Header("String Builder")]
        private StringBuilder ammoTextBuilder;
        private StringBuilder ammoRefillPromptBuilder;

        private void Start()
        {
            inventory = GetComponent<PlayerInventory>();
            interact = GetComponent<PlayerInteract>();
            ammoTextBuilder = new StringBuilder();
            ammoRefillPromptBuilder = new StringBuilder();
        }
        private void Update()
        {
            currentWeapon = inventory.CurrentWeapon;
            UpdateAmmoText();
        }
        public void UpdateText(string prompt)
        {
            promptText.text = prompt;
        }
        private void UpdateAmmoText()
        {
            ammoTextBuilder.Clear();

            if (currentWeapon.gunStyle != GunStyle.Melee)
            {
                if (currentWeapon.gunStyle == GunStyle.Grenade || currentWeapon.gunStyle == GunStyle.Flashbang || currentWeapon.gunStyle == GunStyle.Smoke)
                    ammoTextBuilder.Append(currentWeapon.currentAmmoCount.ToString());
                else
                    ammoTextBuilder.Append($"{currentWeapon.currentAmmoCount} / {currentWeapon.maxAmmoCount}");
            }
            else
                ammoTextBuilder.Append("");

            ammoText.text = ammoTextBuilder.ToString();
        }
        public void ShowGrenadePrompt(string gunName)
        {
            ammoRefillPromptBuilder.Insert(0, $"You cannot carry more {gunName}s!\n");
            UpdateAmmoRefillPrompt();
        }
        public void ShowAmmoRefillPrompt(string gunName)
        {
            ammoRefillPromptBuilder.Insert(0, $"You cannot carry more {gunName} ammo!\n");
            UpdateAmmoRefillPrompt();
        }
        public IEnumerator RefillAmmo()
        {
            if (ammoRefill != null)
            {
                ammoRefill.isFilling = true;
                ammoRefill.loadingSlider.SetActive(true);
                float startTime = Time.time;

                while (Time.time - startTime < 2.0f)
                {
                    if (!Physics.Raycast(interact.ray, out interact.hitInfo, interact.distance))
                    {
                        ammoRefill.isFilling = false;
                        ammoRefill.loadingSlider.SetActive(false);

                        yield break;
                    }

                    float progress = (Time.time - startTime) / 2.0f;
                    ammoRefill.slider.value = progress;
                    ammoRefill.sliderValue.text = string.Format("{0:F1}", progress * 2.0f);

                    yield return new WaitForSeconds(0.1f);
                }

                if (inventory != null)
                {
                    foreach (Gun gun in inventory.weapons)
                    {
                        if (gun != null && gun.gunStyle != GunStyle.Melee && gun.gunStyle != GunStyle.Grenade && gun.gunStyle != GunStyle.Flashbang && gun.gunStyle != GunStyle.Smoke)
                        {
                            if (gun.maxAmmoCount < gun.magazineSize * 3)
                                gun.maxAmmoCount += gun.magazineSize;
                            else
                                ShowAmmoRefillPrompt(gun.gunName);
                        }
                    }
                }

                ammoRefill.loadingSlider.SetActive(false);
                ammoRefill.isFilling = false;
            }
        }
        private void UpdateAmmoRefillPrompt()
        {
            string[] lines = ammoRefillPromptBuilder.ToString().Split('\n');

            if (lines.Length > maxLines)
            {
                ammoRefillPromptBuilder.Clear();

                for (int i = 0; i < maxLines; i++)
                    ammoRefillPromptBuilder.Append(lines[i]).Append("\n");

                ammoRefillPrompt.text = ammoRefillPromptBuilder.ToString();
            }
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);

            hideCoroutine = StartCoroutine(HideAmmoRefillPrompt());
        }
        private IEnumerator HideAmmoRefillPrompt()
        {
            StringBuilder hiddenTextBuilder = new StringBuilder(ammoRefillPrompt.text);
            Color textColor = ammoRefillPrompt.color;
            float startTime = Time.time;
            float endTime = startTime + fadeDuration;

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / fadeDuration;
                textColor.a = Mathf.Lerp(1.0f, 0.0f, t);
                ammoRefillPrompt.text = hiddenTextBuilder.ToString();
                ammoRefillPrompt.color = textColor;

                yield return null;
            }

            hiddenTextBuilder.Length = 0;
            ammoRefillPrompt.text = hiddenTextBuilder.ToString();
            textColor.a = 1.0f;
            ammoRefillPrompt.color = textColor;
            hideCoroutine = null;
        }
    }
}