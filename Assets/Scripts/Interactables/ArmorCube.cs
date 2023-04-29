using UnityEngine;

public class ArmorCube : Interactable
{
    [Header("References")]
    private PlayerHealth armor;

    private void Start()
    {
        armor = FindObjectOfType<PlayerHealth>();
    }
    protected override void Interact()
    {
        if (armor != null)
        {
            if(armor.currentArmor <= 99)
            {
                armor.PickupArmor();
                //Destroy(gameObject);
            }
        }
    }
}
