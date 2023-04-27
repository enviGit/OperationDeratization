using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ammo : Interactable
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI ammoRefillPrompt;
    [SerializeField] private GameObject loadingSlider;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderValue;
    private PlayerInventory inventory;
    private PlayerInteract interact;

    [Header("Ammo")]
    private float fadeDuration = 1.5f;
    private Coroutine hideCoroutine;
    private const int maxLines = 2;
    private bool isFilling = false;
    private int allWeapons = 0;
    private int weaponsFullAmmo = 0;

    private void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        interact = FindObjectOfType<PlayerInteract>();
    }
    private void Update()
    {
        if (isFilling)
            prompt = "";
        else
            prompt = "Refill ammo";
    }
    protected override void Interact()
    {
        if (inventory != null)
        {
            foreach (Gun gun in inventory.weapons)
            {
                if (gun != null && gun.gunStyle != GunStyle.Melee)
                {
                    allWeapons++;

                    if (gun.maxAmmoCount >= gun.magazineSize * 4)
                    {
                        weaponsFullAmmo++;
                        ShowAmmoRefillPrompt(gun.gunName);
                    }
                }
            }
        }
        if ((weaponsFullAmmo == 1 && allWeapons == 1) || (weaponsFullAmmo == 2 && allWeapons == 2))
        {
            allWeapons = 0;
            weaponsFullAmmo = 0;
            return;
        }
        if (!isFilling)
            StartCoroutine(ReloadAmmo());

        allWeapons = 0;
        weaponsFullAmmo = 0;
    }
    private IEnumerator ReloadAmmo()
    {
        isFilling = true;
        loadingSlider.SetActive(true);
        float startTime = Time.time;

        while (Time.time - startTime < 3.0f)
        {
            if (inventory.CurrentWeapon.gunStyle == GunStyle.Melee || !Physics.Raycast(interact.ray, out interact.hitInfo, interact.distance - 1f))
            {
                isFilling = false;
                loadingSlider.SetActive(false);
                yield break;
            }

            float progress = (Time.time - startTime) / 3.0f;
            slider.value = progress;
            sliderValue.text = string.Format("{0:F1}", progress * 3.0f);
            yield return new WaitForSeconds(0.1f);
        }

        if (inventory != null)
        {
            foreach (Gun gun in inventory.weapons)
            {
                if (gun != null && gun.gunStyle != GunStyle.Melee)
                {
                    if (gun.maxAmmoCount < gun.magazineSize * 4)
                        gun.maxAmmoCount += gun.magazineSize;
                    else
                        ShowAmmoRefillPrompt(gun.gunName);
                }
            }
        }

        loadingSlider.SetActive(false);
        isFilling = false;
    }
    private void ShowAmmoRefillPrompt(string gunName)
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
