/*using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private Gun[] weapons;

    private void Start()
    {
        InitializeVariables();
    }
    public void AddItem(Gun newItem)
    {
        int newItemIndex = (int)newItem.gunStyle;

        if (weapons[newItemIndex] != null)
            RemoveItem(newItemIndex);

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
    private void InitializeVariables()
    {
        weapons = new Gun[3];
    }
}
*/