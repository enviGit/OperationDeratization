using UnityEngine;

public class InitializeAmmo : MonoBehaviour
{
    public Gun[] guns = new Gun[6];

    void Start()
    {
        foreach (var gun in guns)
        {
            if (gun.name.Contains("Pistol"))
            {
                gun.magazineSize = 12;
                gun.currentAmmoCount = 12;
                gun.maxAmmoCount = 48;
            }else if (gun.name.Contains("Rifle"))
            {
                gun.magazineSize = 30;
                gun.currentAmmoCount = 30;
                gun.maxAmmoCount = 120;
            }
        }
    }
}