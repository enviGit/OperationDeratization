using UnityEngine;

public class InitializeAmmo : MonoBehaviour
{
    [Header("References")]
    public Gun[] guns = new Gun[7];

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
            else if (gun.name.Contains("Rifle"))
            {
                gun.magazineSize = 30;
                gun.currentAmmoCount = 30;
                gun.maxAmmoCount = 90;
            }
        }
    }
}