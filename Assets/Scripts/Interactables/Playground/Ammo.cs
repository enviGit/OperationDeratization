using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ammo : Interactable
{
    [Header("References")]
    public TextMeshProUGUI ammoRefillPrompt;
    public GameObject loadingSlider;
    public Slider slider;
    public TextMeshProUGUI sliderValue;
    private PlayerInventory inventory;
    private PlayerUI ui;
    public AudioSource lootingSound;

    [Header("Ammo")]
    public bool isFilling = false;
    private int allWeapons = 0;
    private int weaponsFullAmmo = 0;

    private void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        ui = FindObjectOfType<PlayerUI>();
        loadingSlider.SetActive(false);
    }
    private void Update()
    {
        if (isFilling)
            prompt = "";
        else
        {
            lootingSound.Stop();
            prompt = "Refill ammo";
        }
    }
    protected override void Interact()
    {
        if (inventory != null)
        {
            foreach (Gun gun in inventory.weapons)
            {
                if (gun != null && gun.gunStyle != GunStyle.Melee && gun.gunStyle != GunStyle.Grenade && gun.gunStyle != GunStyle.Flashbang && gun.gunStyle != GunStyle.Smoke)
                {
                    allWeapons++;

                    if (gun.maxAmmoCount >= gun.magazineSize * 3)
                    {
                        weaponsFullAmmo++;
                        ui.ShowAmmoRefillPrompt(gun.gunName);
                    }
                }
            }
        }
        if (weaponsFullAmmo == allWeapons)
        {
            allWeapons = 0;
            weaponsFullAmmo = 0;
            return;
        }
        if (!isFilling)
        {
            lootingSound.Play();
            StartCoroutine(ui.RefillAmmo());
        }
            

        allWeapons = 0;
        weaponsFullAmmo = 0;
    }
    
}
