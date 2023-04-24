using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI promptText;
    [SerializeField]
    private TextMeshProUGUI ammoText;
    private Gun currentWeapon;

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
            ammoText.text = currentWeapon.currentAmmoCount + " / " + currentWeapon.maxAmmoCount;
        else
            ammoText.text = "";
    }
}
