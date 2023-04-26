using System.Collections;
using UnityEngine;
using TMPro;

public class Ammo : Interactable
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI ammoRefillPrompt;

    [Header("Ammo")]
    private float fadeDuration = 1.5f;
    private Coroutine hideCoroutine;
    private const int maxLines = 2;

    protected override void Interact()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

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
