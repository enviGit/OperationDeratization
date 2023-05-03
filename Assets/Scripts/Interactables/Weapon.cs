using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Weapon : Interactable
{
    [Header("References")]
    public Gun gun;
    [SerializeField] private Image upperImage;
    [SerializeField] private Image bottomImage;
    private PlayerInteract interact;
    private PlayerInventory inventory;
    private PlayerShoot shoot;
    MeshSockets sockets;

    private void Start()
    {
        interact = FindObjectOfType<PlayerInteract>();
        inventory = FindObjectOfType<PlayerInventory>();
        shoot = FindObjectOfType<PlayerShoot>();
    }
    private void Update()
    {
        if (Physics.Raycast(interact.ray, out interact.hitInfo, interact.distance) && interact.hitInfo.transform.GetComponent<Weapon>() && shoot.isAiming == false)
        {
            if (upperImage != null)
            {
                upperImage.gameObject.SetActive(true);
                upperImage.sprite = interact.hitInfo.transform.GetComponent<Weapon>().gun.activeGunIcon;
            }

            prompt = "Pick up " + interact.hitInfo.transform.GetComponent<Weapon>().gun.gunName;

            if (inventory.HasWeaponOfSameCategory(interact.hitInfo.transform.GetComponent<Weapon>().gun) && interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle != GunStyle.Grenade &&
                interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle != GunStyle.Flashbang && interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle != GunStyle.Smoke)
            {
                Gun inventoryWeapon = null;

                foreach (Gun gun in inventory.weapons)
                {
                    if (gun != null && gun.gunStyle == interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle)
                        inventoryWeapon = gun;
                }

                //prompt = "Swap " + inventoryWeapon.gunName + "\n\n\n\nfor " + interact.hitInfo.transform.GetComponent<Weapon>().gun.gunName;
                prompt = "Swap\n\n\n\nfor";

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
            SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                skinnedMeshRenderer.receiveShadows = false;
            }
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
            }
        }

        int childIndex = 0;

        if (gun.gunStyle == GunStyle.Melee)
            childIndex = 0;
        else if (gun.gunStyle == GunStyle.Primary)
            childIndex = 1;
        else if (gun.gunStyle == GunStyle.Secondary)
            childIndex = 2;
        else if (gun.gunStyle == GunStyle.Grenade)
            childIndex = 3;
        else if (gun.gunStyle == GunStyle.Flashbang)
            childIndex = 4;
        else
            childIndex = 5;

        weaponObject.transform.SetSiblingIndex(childIndex);
        Destroy(gameObject);
        inventory.SetCurrentWeapon(Array.IndexOf(inventory.weapons, gun));
        inventory.UpdateWeaponImages();
    }
    private void OnTriggerEnter(Collider other)
    {
        AiWeapons weapons = other.gameObject.GetComponent<AiWeapons>();
        sockets = other.gameObject.GetComponentInChildren<MeshSockets>();

        if (weapons != null && weapons.GetComponent<AiAgent>().stateMachine.currentState != AiStateId.Death)
        {
            GameObject newWeapon = Instantiate(gun.gunPrefab);
            weapons.Equip(newWeapon, sockets);
            Destroy(gameObject);
        }
    }
    /*private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Destroy(GetComponent<Rigidbody>());
            Collider collider = GetComponent<Collider>();

            if (collider != null)
                collider.isTrigger = true;
        }
    }*/
}
