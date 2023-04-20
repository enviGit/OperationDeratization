using System;
using UnityEngine;

public class Weapon : Interactable
{
    [SerializeField]
    private Gun gun;

    protected override void Interact()
    {
        prompt = "Pick up weapon";
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        inventory.AddItem(gun);
        GameObject weaponObject = Instantiate(gun.gunPrefab, Vector3.zero, Quaternion.identity, Camera.main.transform.Find("WeaponHolder"));
        weaponObject.layer = LayerMask.NameToLayer("Player");
        weaponObject.transform.localPosition = Vector3.zero;
        weaponObject.transform.localRotation = Quaternion.identity;
        int childIndex = 1;

        if (gun.gunStyle == GunStyle.Secondary)
            childIndex = 2;

        weaponObject.transform.SetSiblingIndex(childIndex);
        Destroy(gameObject);

        inventory.SetCurrentWeapon(Array.IndexOf(inventory.weapons, gun));
    }
}
