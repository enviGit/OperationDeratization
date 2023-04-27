using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : Interactable
{
    [Header("References")]
    [SerializeField] private Gun gun;

    protected override void Interact()
    {
        prompt = "Pick up " + gun.gunName;
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
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
