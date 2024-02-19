using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Weapon images")]
        [SerializeField] private Image meleeWeaponImage;
        [SerializeField] private Image primaryWeaponImage;
        [SerializeField] private Image secondaryWeaponImage;
        public Image grenadeWeaponImage;
        public Image flashbangWeaponImage;
        public Image smokeWeaponImage;
        public Image molotovWeaponImage;
        private PlayerUI playerUI;
        private PlayerShoot playerShoot;
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private GameObject wheels;

        [Header("Weapon")]
        [SerializeField] private Gun melee;
        public Gun[] weapons;
        public int currentWeaponIndex = -1;
        private int currentItemIndex = 0;
        [HideInInspector] public int grenadeCount = 0;
        [HideInInspector] public int flashbangCount = 0;
        [HideInInspector] public int smokeCount = 0;
        [HideInInspector] public int molotovCount = 0;
        public Gun CurrentWeapon
        {
            get
            {
                if (currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Length)
                    return weapons[currentWeaponIndex];
                else
                    return null;
            }
        }

        private void Start()
        {
            playerShoot = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShoot>();
            playerUI = GetComponent<PlayerUI>();
            weapons = new Gun[7];
            weapons[0] = melee;
            weapons[1] = null;
            weapons[2] = null;
            weapons[3] = null;
            weapons[4] = null;
            weapons[5] = null;
            weapons[6] = null;
            currentWeaponIndex = 0;
            UpdateWeaponImages();
        }
        private void Update()
        {
            SwitchItem();
            RemoveItem();

            if (weapons[3] != null)
                grenadeCount = weapons[3].currentAmmoCount;
            else
                grenadeCount = 0;
            if (weapons[4] != null)
                flashbangCount = weapons[4].currentAmmoCount;
            else
                flashbangCount = 0;
            if (weapons[5] != null)
                smokeCount = weapons[5].currentAmmoCount;
            else
                smokeCount = 0;
            if (weapons[6] != null)
                molotovCount = weapons[6].currentAmmoCount;
            else
                molotovCount = 0;
        }
        public void AddItem(Gun newItem)
        {
            int newItemIndex = (int)newItem.gunStyle;

            if (weapons[newItemIndex] != null)
            {
                if (newItem.gunStyle == GunStyle.Melee)
                {
                    Transform melee = transform.Find("Camera/Main Camera/WeaponHolder/Knife_00(Clone)");

                    if (melee != null)
                        Destroy(melee.gameObject);
                }
                else if (newItem.gunStyle == GunStyle.Primary)
                {
                    Transform pistol = transform.Find("Camera/Main Camera/WeaponHolder/Pistol_00(Clone)");
                    Transform revolver = transform.Find("Camera/Main Camera/WeaponHolder/Revolver_00(Clone)");

                    if (pistol != null)
                        Destroy(pistol.gameObject);
                    if (revolver != null)
                        Destroy(revolver.gameObject);
                }
                else if (newItem.gunStyle == GunStyle.Secondary)
                {
                    Transform shotgun = transform.Find("Camera/Main Camera/WeaponHolder/Shotgun_00(Clone)");
                    Transform rifle = transform.Find("Camera/Main Camera/WeaponHolder/Rifle_00(Clone)");
                    Transform sniper = transform.Find("Camera/Main Camera/WeaponHolder/Sniper_00(Clone)");

                    if (shotgun != null)
                        Destroy(shotgun.gameObject);
                    if (rifle != null)
                        Destroy(rifle.gameObject);
                    if (sniper != null)
                        Destroy(sniper.gameObject);
                }
                else if (newItem.gunStyle == GunStyle.Grenade)
                {
                    Transform grenade = transform.Find("Camera/Main Camera/WeaponHolder/Grenade_00(Clone)");

                    if (grenade != null)
                        Destroy(grenade.gameObject);
                    if (newItem.currentAmmoCount < newItem.editorAmmoValue)
                        newItem.currentAmmoCount = newItem.editorAmmoValue;
                    else
                        playerUI.ShowGrenadePrompt(newItem.gunName);
                }
                else if (newItem.gunStyle == GunStyle.Flashbang)
                {
                    Transform flashbang = transform.Find("Camera/Main Camera/WeaponHolder/Flashbang_00(Clone)");

                    if (flashbang != null)
                        Destroy(flashbang.gameObject);
                    if (newItem.currentAmmoCount < newItem.editorAmmoValue)
                        newItem.currentAmmoCount = newItem.editorAmmoValue;
                    else
                        playerUI.ShowGrenadePrompt(newItem.gunName);
                }
                else if (newItem.gunStyle == GunStyle.Smoke)
                {
                    Transform smoke = transform.Find("Camera/Main Camera/WeaponHolder/Smoke_00(Clone)");

                    if (smoke != null)
                        Destroy(smoke.gameObject);
                    if (newItem.currentAmmoCount < newItem.editorAmmoValue)
                        newItem.currentAmmoCount = newItem.editorAmmoValue;
                    else
                        playerUI.ShowGrenadePrompt(newItem.gunName);
                }
                else if (newItem.gunStyle == GunStyle.Molotov)
                {
                    Transform molotov = transform.Find("Camera/Main Camera/WeaponHolder/Molotov_00(Clone)");

                    if (molotov != null)
                        Destroy(molotov.gameObject);
                    if (newItem.currentAmmoCount < newItem.editorAmmoValue)
                        newItem.currentAmmoCount = newItem.editorAmmoValue;
                    else
                        playerUI.ShowGrenadePrompt(newItem.gunName);
                }
                if (newItem.gunStyle == GunStyle.Primary || newItem.gunStyle == GunStyle.Secondary)
                {
                    Vector3 dropPosition = transform.position + transform.forward * 0.5f + transform.up * 1f;
                    GameObject newWeapon = Instantiate(weapons[newItemIndex].gunPrefab, dropPosition, Quaternion.identity);
                    newWeapon.layer = LayerMask.NameToLayer("Interactable");
                    SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
                    newWeapon.tag = "Weapon";
                    Rigidbody weaponRigidbody = newWeapon.AddComponent<Rigidbody>();
                    weaponRigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
                    weaponRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    weaponRigidbody.mass = 2f;
                    Quaternion randomRotation = Random.rotation;
                    newWeapon.transform.rotation = randomRotation;
                }
            }

            weapons[newItemIndex] = newItem;
            UpdateWeaponImages();
        }
        public void SwitchItem()
        {
            if (playerShoot.isAiming == false && !wheels.activeSelf)
            {
                int scrollDelta = (int)Input.mouseScrollDelta.y;

                if (scrollDelta != 0)
                {
                    int newWeaponIndex = FindNextWeaponIndex(scrollDelta);

                    if (newWeaponIndex != currentWeaponIndex)
                    {
                        SetCurrentWeapon(newWeaponIndex);
                        UpdateWeaponImages();
                        currentItemIndex = newWeaponIndex - 3;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SetCurrentWeapon(0);
                    UpdateWeaponImages();
                    currentItemIndex = -1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SetCurrentWeapon(1);
                    UpdateWeaponImages();
                    currentItemIndex = -1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    SetCurrentWeapon(2);
                    UpdateWeaponImages();
                    currentItemIndex = -1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    currentItemIndex++;

                    if (currentItemIndex > 3)
                        currentItemIndex = 0;

                    int newWeaponIndex = currentItemIndex + 3;

                    while (weapons[newWeaponIndex] == null && currentItemIndex < 4)
                    {
                        currentItemIndex++;
                        newWeaponIndex = currentItemIndex + 3;
                    }

                    SetCurrentWeapon(newWeaponIndex);
                    UpdateWeaponImages();
                }
            }
        }
        private int FindNextWeaponIndex(int scrollDelta)
        {
            int newIndex = currentWeaponIndex;

            do
            {
                newIndex = (newIndex + scrollDelta) % weapons.Length;

                if (newIndex < 0)
                    newIndex += weapons.Length;
            }
            while (weapons[newIndex] == null && newIndex != currentWeaponIndex);

            return newIndex;
        }
        public void RemoveItem()
        {
            if (Input.GetKeyDown(KeyCode.G) && (CurrentWeapon.gunStyle == GunStyle.Primary || CurrentWeapon.gunStyle == GunStyle.Secondary))
            {
                Gun droppedWeapon = CurrentWeapon;
                weapons[currentWeaponIndex] = null;

                if (droppedWeapon != null)
                {
                    Image weaponImage = null;

                    switch (droppedWeapon.gunStyle)
                    {
                        case GunStyle.Melee:
                            weaponImage = meleeWeaponImage;
                            break;
                        case GunStyle.Primary:
                            weaponImage = primaryWeaponImage;
                            break;
                        case GunStyle.Secondary:
                            weaponImage = secondaryWeaponImage;
                            break;
                        case GunStyle.Grenade:
                            weaponImage = grenadeWeaponImage;
                            break;
                        case GunStyle.Flashbang:
                            weaponImage = flashbangWeaponImage;
                            break;
                        case GunStyle.Smoke:
                            weaponImage = smokeWeaponImage;
                            break;
                        case GunStyle.Molotov:
                            weaponImage = molotovWeaponImage;
                            break;
                    }

                    if (weaponImage != null)
                        weaponImage.gameObject.SetActive(false);

                    Vector3 dropPosition = transform.position + transform.forward * 0.5f + transform.up * 1f;
                    GameObject newWeapon = Instantiate(droppedWeapon.gunPrefab, dropPosition, Quaternion.identity);
                    newWeapon.layer = LayerMask.NameToLayer("Interactable");
                    SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
                    newWeapon.tag = "Weapon";
                    Rigidbody weaponRigidbody = newWeapon.AddComponent<Rigidbody>();
                    weaponRigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
                    weaponRigidbody.mass = 2f;
                    weaponRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    Quaternion randomRotation = Random.rotation;
                    newWeapon.transform.rotation = randomRotation;

                    foreach (Transform child in weaponHolder)
                    {
                        if (child.gameObject.name == droppedWeapon.gunPrefab.name + "(Clone)")
                        {
                            Destroy(child.gameObject);

                            break;
                        }
                    }
                }

                SetCurrentWeapon(0);
                UpdateWeaponImages();
            }
        }
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
        public void SetCurrentWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length || weapons[index] == null)
                return;

            foreach (Transform weapon in weaponHolder)
            {
                if (weapon.gameObject.name != weapons[index].gunPrefab.name + "(Clone)")
                    weapon.gameObject.SetActive(false);
                else
                    weapon.gameObject.SetActive(true);

                weapon.tag = "Untagged";
            }

            currentWeaponIndex = index;

            if (weapons[currentWeaponIndex].gunType == GunType.Sniper)
                playerShoot.sniperCam = transform.Find("Camera/Main Camera/WeaponHolder/" + weapons[currentWeaponIndex].gunPrefab.name + "(Clone)/Mesh/SVD/Camera").GetComponent<Camera>();
        }
        public void UpdateWeaponImages()
        {
            if (meleeWeaponImage != null)
            {
                if (CurrentWeapon.gunStyle != GunStyle.Melee)
                    meleeWeaponImage.sprite = weapons[0].gunIcon;
                else
                {
                    meleeWeaponImage.sprite = CurrentWeapon.activeGunIcon;
                    meleeWeaponImage.gameObject.SetActive(true);
                }
            }
            if (primaryWeaponImage != null)
            {
                if (CurrentWeapon.gunStyle != GunStyle.Primary)
                {
                    if (weapons[1] != null)
                        primaryWeaponImage.sprite = weapons[1].gunIcon;
                }
                else
                {
                    primaryWeaponImage.sprite = CurrentWeapon.activeGunIcon;
                    primaryWeaponImage.gameObject.SetActive(true);
                }
            }
            if (secondaryWeaponImage != null)
            {
                if (CurrentWeapon.gunStyle != GunStyle.Secondary)
                {
                    if (weapons[2] != null)
                        secondaryWeaponImage.sprite = weapons[2].gunIcon;
                }
                else
                {
                    secondaryWeaponImage.sprite = CurrentWeapon.activeGunIcon;
                    secondaryWeaponImage.gameObject.SetActive(true);
                }
            }
            if (grenadeWeaponImage != null)
            {
                if (CurrentWeapon.gunStyle != GunStyle.Grenade)
                {
                    if (weapons[3] != null)
                        grenadeWeaponImage.sprite = weapons[3].gunIcon;
                }
                else
                {
                    grenadeWeaponImage.sprite = CurrentWeapon.activeGunIcon;
                    grenadeWeaponImage.gameObject.SetActive(true);
                }
            }
            if (flashbangWeaponImage != null)
            {
                if (CurrentWeapon.gunStyle != GunStyle.Flashbang)
                {
                    if (weapons[4] != null)
                        flashbangWeaponImage.sprite = weapons[4].gunIcon;
                }
                else
                {
                    flashbangWeaponImage.sprite = CurrentWeapon.activeGunIcon;
                    flashbangWeaponImage.gameObject.SetActive(true);
                }
            }
            if (smokeWeaponImage != null)
            {
                if (CurrentWeapon.gunStyle != GunStyle.Smoke)
                {
                    if (weapons[5] != null)
                        smokeWeaponImage.sprite = weapons[5].gunIcon;
                }
                else
                {
                    smokeWeaponImage.sprite = CurrentWeapon.activeGunIcon;
                    smokeWeaponImage.gameObject.SetActive(true);
                }
            }
            if (molotovWeaponImage != null)
            {
                if (CurrentWeapon.gunStyle != GunStyle.Molotov)
                {
                    if (weapons[6] != null)
                        molotovWeaponImage.sprite = weapons[6].gunIcon;
                }
                else
                {
                    molotovWeaponImage.sprite = CurrentWeapon.activeGunIcon;
                    molotovWeaponImage.gameObject.SetActive(true);
                }
            }
        }
        public bool HasWeaponOfSameCategory(Gun newGun)
        {
            foreach (Gun gun in weapons)
            {
                if (gun != null && gun.gunStyle == newGun.gunStyle)
                    return true;
            }

            return false;
        }
    }
}