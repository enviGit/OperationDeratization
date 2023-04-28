using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Weapon images")]
    [SerializeField] private Image meleeWeaponImage;
    [SerializeField] private Image primaryWeaponImage;
    [SerializeField] private Image secondaryWeaponImage;

    [Header("Weapon")]
    [SerializeField] private Gun melee;
    public Gun[] weapons;
    private int currentWeaponIndex = -1;
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
        Transform mesh = transform.Find("Camera/Main Camera/WeaponHolder/Knife_00(Clone)");
        MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        weapons = new Gun[3];
        weapons[0] = melee;
        weapons[1] = null;
        weapons[2] = null;
        currentWeaponIndex = 0;
        UpdateWeaponImages();
    }

    private void Update()
    {
        SwitchItem();
        RemoveItem();
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

            Vector3 dropPosition = transform.position + transform.forward * 0.5f + transform.up * 1f;
            GameObject newWeapon = Instantiate(weapons[newItemIndex].gunPrefab, dropPosition, Quaternion.identity);
            newWeapon.layer = LayerMask.NameToLayer("Interactable");
            SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
            Rigidbody weaponRigidbody = newWeapon.AddComponent<Rigidbody>();
            weaponRigidbody.mass = 5f;
            weaponRigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
            Quaternion randomRotation = Random.rotation;
            newWeapon.transform.rotation = randomRotation;
        }

        weapons[newItemIndex] = newItem;
        UpdateWeaponImages();
    }
    public void SwitchItem()
    {
        int scrollDelta = (int)Input.mouseScrollDelta.y;

        if (scrollDelta != 0)
        {
            int newWeaponIndex = (currentWeaponIndex + scrollDelta) % weapons.Length;

            if (newWeaponIndex < 0)
                newWeaponIndex += weapons.Length;

            SetCurrentWeapon(newWeaponIndex);
            UpdateWeaponImages();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCurrentWeapon(0);
            UpdateWeaponImages();
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCurrentWeapon(1);
            UpdateWeaponImages();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCurrentWeapon(2);
            UpdateWeaponImages();
        }
    }
    public void RemoveItem()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentWeaponIndex != 0)
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
                }
                if (weaponImage != null)
                    weaponImage.gameObject.SetActive(false);

                Vector3 dropPosition = transform.position + transform.forward * 0.5f + transform.up * 1f;
                GameObject newWeapon = Instantiate(droppedWeapon.gunPrefab, dropPosition, Quaternion.identity);
                newWeapon.layer = LayerMask.NameToLayer("Interactable");
                SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
                Rigidbody weaponRigidbody = newWeapon.AddComponent<Rigidbody>();
                weaponRigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
                weaponRigidbody.mass = 5f;
                Quaternion randomRotation = Random.rotation;
                newWeapon.transform.rotation = randomRotation;
                Transform weaponHolder = transform.Find("Camera/Main Camera/WeaponHolder");

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

        Transform weaponHolder = transform.Find("Camera/Main Camera/WeaponHolder");

        foreach (Transform weapon in weaponHolder)
        {
            if (weapon.gameObject.name != weapons[index].gunPrefab.name + "(Clone)")
                weapon.gameObject.SetActive(false);
            else
                weapon.gameObject.SetActive(true);
        }

        currentWeaponIndex = index;
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
                if(weapons[1] != null)
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
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        if (collidedObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            Rigidbody collidedRigidbody = collidedObject.GetComponent<Rigidbody>();

            if (collidedRigidbody != null)
                Destroy(collidedRigidbody);

            collidedObject.transform.position = collision.contacts[0].point;
            collidedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);
        }
    }
}