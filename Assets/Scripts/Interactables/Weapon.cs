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
        weaponObject.transform.localPosition = Vector3.zero;
        weaponObject.transform.localRotation = Quaternion.identity;
        weaponObject.transform.localScale = Vector3.one;
        Destroy(gameObject);
    }
}
