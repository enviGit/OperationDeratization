using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Weapon : Interactable
{
    [Header("References")]
    [SerializeField] private Gun gun;
    [SerializeField] private Image upperImage;
    [SerializeField] private Image bottomImage;
    private PlayerInteract interact;
    private PlayerInventory inventory;
    private PlayerMotor motor;

    private void Start()
    {
        interact = FindObjectOfType<PlayerInteract>();
        inventory = FindObjectOfType<PlayerInventory>();
        motor = FindObjectOfType<PlayerMotor>();
    }
    private void Update()
    {
        if (Physics.Raycast(interact.ray, out interact.hitInfo, interact.distance) && interact.hitInfo.transform.GetComponent<Weapon>() && motor.isAiming == false)
        {
            if (upperImage != null)
            {
                upperImage.gameObject.SetActive(true);
                upperImage.sprite = interact.hitInfo.transform.GetComponent<Weapon>().gun.activeGunIcon;
            }

            prompt = "Pick up " + interact.hitInfo.transform.GetComponent<Weapon>().gun.gunName;

            if (inventory.HasWeaponOfSameCategory(interact.hitInfo.transform.GetComponent<Weapon>().gun))
            {
                Gun inventoryWeapon = null;

                foreach (Gun gun in inventory.weapons)
                {
                    if (gun != null && gun.gunStyle == interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle)
                        inventoryWeapon = gun;
                }
                prompt = "Swap " + inventoryWeapon.gunName + "\n\n\nfor " + interact.hitInfo.transform.GetComponent<Weapon>().gun.gunName;

                if (upperImage != null)
                {
                    upperImage.gameObject.SetActive(true);
                    upperImage.sprite = inventoryWeapon.activeGunIcon;
                }
                if (bottomImage != null)
                {
                    bottomImage.gameObject.SetActive(true);
                    bottomImage.sprite = interact.hitInfo.transform.GetComponent<Weapon>().gun.activeGunIcon;
                }
            }
        }
        else
        {
            if (upperImage != null || bottomImage != null)
            {
                upperImage.gameObject.SetActive(false);
                bottomImage.gameObject.SetActive(false);
                prompt = "";
            } 
        }
    }
    protected override void Interact()
    {
        inventory.AddItem(gun);
        GameObject weaponObject = Instantiate(gun.gunPrefab, Vector3.zero, Quaternion.identity, Camera.main.transform.Find("WeaponHolder"));
        weaponObject.layer = LayerMask.NameToLayer("Player");
        var script = weaponObject.GetComponent<Weapon>();
        Destroy(script);
        weaponObject.transform.localPosition = Vector3.zero;
        weaponObject.transform.localRotation = Quaternion.identity;
        Transform mesh = weaponObject.transform.Find("Mesh");

        foreach (Transform child in mesh)
        {
            SkinnedMeshRenderer meshRenderer = child.GetComponent<SkinnedMeshRenderer>();

            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
            }
        }

        int childIndex = 1;

        if (gun.gunStyle == GunStyle.Secondary)
            childIndex = 2;

        weaponObject.transform.SetSiblingIndex(childIndex);
        Destroy(gameObject);
        inventory.SetCurrentWeapon(Array.IndexOf(inventory.weapons, gun));
        inventory.UpdateWeaponImages();
    }
}
