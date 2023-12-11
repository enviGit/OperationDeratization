using System;
using System.Collections;
using System.Collections.Generic;
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
        GameObject upperImageObject = GameObject.FindGameObjectWithTag("UpperImage");
        GameObject bottomImageObject = GameObject.FindGameObjectWithTag("BottomImage");

        if (upperImageObject != null)
            upperImage = upperImageObject.GetComponent<Image>();
        if (bottomImageObject != null)
            bottomImage = bottomImageObject.GetComponent<Image>();
    }
    private void Update()
    {
        if (Physics.Raycast(interact.ray, out interact.hitInfo, interact.distance) && interact.hitInfo.transform.GetComponent<Weapon>() && shoot.isAiming == false)
        {
            if (upperImage != null)
            {
                if (interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle != GunStyle.Grenade && interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle != GunStyle.Flashbang && interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle != GunStyle.Smoke)
                {
                    upperImage.gameObject.SetActive(true);
                    upperImage.sprite = interact.hitInfo.transform.GetComponent<Weapon>().gun.activeGunIcon;
                }

            }
            if (interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Grenade)
                prompt = "Refill Explosive Grenades";
            else if (interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Flashbang)
                prompt = "Refill Flashbang Grenades";
            else if (interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Smoke)
                prompt = "Refill Smoke Grenades";
            else
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
        Transform parent = transform.parent;

        while (parent != null)
        {
            if (parent.CompareTag("Enemy"))
                return;

            parent = parent.parent;
        }

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
        inventory.SetCurrentWeapon(Array.IndexOf(inventory.weapons, gun));
        inventory.UpdateWeaponImages();

        if ((interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Grenade && inventory.grenadeCount < 3) ||
    (interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Flashbang && inventory.flashbangCount < 3) ||
    (interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Smoke && inventory.smokeCount < 3))
        {
            List<Transform> grenadeObjects = new List<Transform>();

            foreach (Transform child in interact.hitInfo.transform)
                grenadeObjects.Add(child);

            if (grenadeObjects.Count >= 3)
            {
                List<Transform> selectedGrenades = new List<Transform>();
                List<Transform> usedGrenades = new List<Transform>();
                grenadeObjects.Shuffle();

                foreach (Transform grenade in grenadeObjects)
                {
                    if (selectedGrenades.Count >= 3)
                        break;
                    if (!usedGrenades.Contains(grenade))
                    {
                        bool dissolveSet = false;

                        foreach (Transform child in grenade.GetChild(0))
                        {
                            MeshRenderer renderer = child.GetComponent<MeshRenderer>();

                            if (renderer != null)
                            {
                                float dissolveValue = renderer.material.GetFloat("_dissolve");

                                if (dissolveValue < 1f)
                                {
                                    StartCoroutine(DestroyAfterPickup(renderer));
                                    dissolveSet = true;
                                }
                            }
                        }
                        if (dissolveSet)
                        {
                            selectedGrenades.Add(grenade);
                            usedGrenades.Add(grenade);
                        }
                    }
                }
            }
        }
        else if (interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Primary || interact.hitInfo.transform.GetComponent<Weapon>().gun.gunStyle == GunStyle.Secondary)
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator DestroyAfterPickup(MeshRenderer mesh)
    {
        SetShaderParameters(0, mesh);
        float elapsedTime = 0f;
        float duration = 0.45f;

        while (elapsedTime < duration)
        {
            SetShaderParameters(elapsedTime / duration, mesh);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if(mesh != null)
            Destroy(mesh.transform.parent.parent.gameObject);
    }
    private void SetShaderParameters(float disappearIntensity, MeshRenderer mesh)
    {
        MeshRenderer meshRenderer = mesh;

        if (meshRenderer != null)
            meshRenderer.material.SetFloat("_dissolve", disappearIntensity);
    }
    private void OnTriggerEnter(Collider other)
    {
        AiWeapons weapons = other.gameObject.GetComponent<AiWeapons>();
        sockets = other.gameObject.GetComponentInChildren<MeshSockets>();

        if (weapons != null && weapons.GetComponent<AiAgent>().stateMachine.currentState != AiStateId.Death)
        {
            if (gun.gunStyle != GunStyle.Grenade && gun.gunStyle != GunStyle.Flashbang && gun.gunStyle != GunStyle.Smoke && weapons.currentWeapon == null)
            {
                GameObject newWeapon = Instantiate(gun.gunPrefab);
                weapons.Equip(newWeapon, sockets);
                Destroy(gameObject);
            }
        }
    }
}
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}