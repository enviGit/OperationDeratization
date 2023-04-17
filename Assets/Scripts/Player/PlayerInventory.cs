using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private Gun fists;
    [SerializeField]
    private Gun[] weapons;
    private int currentWeaponIndex = -1;

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

            foreach (Transform child in transform.Find("Main Camera/WeaponHolder")) //Doesn't do what it should
                child.gameObject.SetActive(false);                                  //Doesn't do what it should

            Vector3 dropPosition = transform.position + transform.forward;
            GameObject newWeapon = Instantiate(weapons[newItemIndex].gunPrefab, dropPosition, Quaternion.identity);
            newWeapon.layer = LayerMask.NameToLayer("Interactable");
            SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
            RemoveItem(newItemIndex);
            Rigidbody weaponRigidbody = newWeapon.AddComponent<Rigidbody>();
            weaponRigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
        }

        weapons[newItemIndex] = newItem;
    }
    public void RemoveItem(int index)
    {
        weapons[index] = null;
    }
    public Gun GetItem(int index)
    {
        return weapons[index];
    }
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
    private void SetCurrentWeapon(int index)
    {
        //old
        /*if (index < 0 || index >= weapons.Length || weapons[index] == null)
            return;

        if (currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Length)
        {
            Transform weaponHolder = transform.Find("Main Camera/WeaponHolder");
            Transform oldWeapon = weaponHolder.GetChild(currentWeaponIndex);

            if (oldWeapon != null)
                oldWeapon.gameObject.SetActive(false);
        }

        Transform newWeapon = transform.Find("Main Camera/WeaponHolder/" + weapons[index].gunPrefab.name + "(Clone)");

        if (newWeapon != null)
            newWeapon.gameObject.SetActive(true);

        currentWeaponIndex = index;*/



        //new
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