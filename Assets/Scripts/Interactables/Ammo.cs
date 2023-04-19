using UnityEngine;

public class Ammo : Interactable
{
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
                }
            }
        }
    }
}