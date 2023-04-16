using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private Gun[] weapons;

    private void Start()
    {
        weapons = new Gun[3];
    }
    public void AddItem(Gun newItem)
    {
        int newItemIndex = (int)newItem.gunStyle;

        if (weapons[newItemIndex] != null)
        {
            Transform weaponHolder = transform.Find("Main Camera/WeaponHolder");

            if (weaponHolder.childCount > 0)
                Destroy(weaponHolder.GetChild(0).gameObject);

            Vector3 dropPosition = transform.position + transform.forward;
            GameObject newWeapon = Instantiate(weapons[newItemIndex].gunPrefab, dropPosition, Quaternion.identity);
            newWeapon.layer = LayerMask.NameToLayer("Interactable");
            SetLayerRecursively(newWeapon, LayerMask.NameToLayer("Interactable"));
            RemoveItem(newItemIndex);
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
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}