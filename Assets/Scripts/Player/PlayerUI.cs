using UnityEngine;
using TMPro;
using System.Collections;

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

    private void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        interact = FindObjectOfType<PlayerInteract>();
    }
    private void Update()
    {
        currentWeapon = GetComponent<PlayerInventory>().CurrentWeapon;
        UpdateAmmoText();
    }
    public void UpdateText(string prompt)
    {
        promptText.text = prompt;
    }
    private void UpdateAmmoText()
    {
        if (currentWeapon.gunStyle != GunStyle.Melee)
        {
            if (currentWeapon.gunStyle == GunStyle.Grenade || currentWeapon.gunStyle == GunStyle.Flashbang || currentWeapon.gunStyle == GunStyle.Smoke)
                ammoText.text = currentWeapon.currentAmmoCount.ToString();
            else
                ammoText.text = currentWeapon.currentAmmoCount + " / " + currentWeapon.maxAmmoCount;
        }
        else
            ammoText.text = "";
    }
    public void ShowGrenadePrompt(string gunName)
    {
        ammoRefillPrompt.text = "You cannot carry more " + gunName + "s!\n" + ammoRefillPrompt.text;
        string[] lines = ammoRefillPrompt.text.Split('\n');

        if (lines.Length > maxLines)
        {
            string newText = "";

            for (int i = 0; i < maxLines; i++)
                newText += lines[i] + "\n";

            ammoRefillPrompt.text = newText;
        }
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAmmoRefillPrompt());
    }
    public IEnumerator RefillAmmo()
    {
        if(ammoRefill != null)
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
    public void ShowAmmoRefillPrompt(string gunName)
    {
        ammoRefillPrompt.text = "You cannot carry more " + gunName + " ammo!\n" + ammoRefillPrompt.text;
        string[] lines = ammoRefillPrompt.text.Split('\n');

        if (lines.Length > maxLines)
        {
            string newText = "";

            for (int i = 0; i < maxLines; i++)
                newText += lines[i] + "\n";

            ammoRefillPrompt.text = newText;
        }
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAmmoRefillPrompt());
    }
    private IEnumerator HideAmmoRefillPrompt()
    {
        Color textColor = ammoRefillPrompt.color;
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / fadeDuration;
            textColor.a = Mathf.Lerp(1.0f, 0.0f, t);
            ammoRefillPrompt.color = textColor;
            yield return null;
        }

        ammoRefillPrompt.text = "";
        textColor.a = 1.0f;
        ammoRefillPrompt.color = textColor;
        hideCoroutine = null;
    }
}
