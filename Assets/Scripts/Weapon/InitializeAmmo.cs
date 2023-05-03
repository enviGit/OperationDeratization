using UnityEngine;

public class InitializeAmmo : MonoBehaviour
{
    [Header("References")]
    public Gun[] guns = new Gun[12];

    private void Start()
    {
        foreach (var gun in guns)
        {
            if (gun.name.Contains("Pistol"))
            {
                gun.magazineSize = 12;
                gun.currentAmmoCount = 12;
                gun.maxAmmoCount = 36;
            }
            else if (gun.name.Contains("Revolver"))
            {
                gun.magazineSize = 6;
                gun.currentAmmoCount = 6;
                gun.maxAmmoCount = 18;
            }
            else if (gun.name.Contains("Shotgun"))
            {
                gun.magazineSize = 7;
                gun.currentAmmoCount = 7;
                gun.maxAmmoCount = 21;
            }
            else if (gun.name.Contains("Rifle"))
            {
                gun.magazineSize = 30;
                gun.currentAmmoCount = 30;
                gun.maxAmmoCount = 90;
            }
            else if (gun.name.Contains("Sniper"))
            {
                gun.magazineSize = 5;
                gun.currentAmmoCount = 5;
                gun.maxAmmoCount = 15;
            }
            else if (gun.name.Contains("Grenade") || gun.name.Contains("Flashbang") || gun.name.Contains("Smoke"))
            {
                gun.magazineSize = 1;
                gun.currentAmmoCount = 1;
                gun.maxAmmoCount = 3;
            }
        }
    }
}