using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private Gun fists;
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
        weapons = new Gun[3];
        weapons[0] = fists;
        weapons[1] = null;
        weapons[2] = null;
        currentWeaponIndex = 0;
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
            if (newItem.gunStyle == GunStyle.Primary)
            {
                Transform pistol = transform.Find("Main Camera/WeaponHolder/Pistol_00(Clone)");

                if (pistol != null)
                    Destroy(pistol.gameObject);
            }
            else if (newItem.gunStyle == GunStyle.Secondary)
            {
                Transform rifle = transform.Find("Main Camera/WeaponHolder/Rifle_00(Clone)");

                if (rifle != null)
                    Destroy(rifle.gameObject);
            }
            else if (newItem.gunStyle == GunStyle.Melee)
            {
                Transform melee = transform.Find("Main Camera/WeaponHolder/Fists(Clone)");

                if (melee != null)
                    Destroy(melee.gameObject);
            }

            Vector3 dropPosition = transform.position + transform.forward * 0.5f + transform.up * 1f;
            GameObject newWeapon = Instantiate(weapons[newItemIndex].gunPrefab, dropPosition, Quaternion.identity);
            newWeapon.layer = LayerMask.NameToLayer("Interactable");
            SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
            Rigidbody weaponRigidbody = newWeapon.AddComponent<Rigidbody>();
            weaponRigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
        }

        weapons[newItemIndex] = newItem;
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
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetCurrentWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCurrentWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetCurrentWeapon(2);
    }
    public void RemoveItem()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentWeaponIndex != 0)
        {
            Gun droppedWeapon = CurrentWeapon;
            weapons[currentWeaponIndex] = null;

            if (droppedWeapon != null)
            {
                Vector3 dropPosition = transform.position + transform.forward * 0.5f + transform.up * 1f;
                GameObject newWeapon = Instantiate(droppedWeapon.gunPrefab, dropPosition, Quaternion.identity);
                newWeapon.layer = LayerMask.NameToLayer("Interactable");
                SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
                Rigidbody weaponRigidbody = newWeapon.AddComponent<Rigidbody>();
                weaponRigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
                Transform weaponHolder = transform.Find("Main Camera/WeaponHolder");

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

        Transform weaponHolder = transform.Find("Main Camera/WeaponHolder");

        foreach (Transform weapon in weaponHolder)
        {
            if (weapon.gameObject.name != weapons[index].gunPrefab.name + "(Clone)")
                weapon.gameObject.SetActive(false);
            else
                weapon.gameObject.SetActive(true);
        }

        currentWeaponIndex = index;
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