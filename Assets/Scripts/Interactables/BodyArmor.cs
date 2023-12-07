using UnityEngine;

public class BodyArmor : Interactable
{
    [Header("References")]
    public PlayerHealth playerArmor;
    private AudioSource pickingArmorSound;

    private void Start()
    {
        pickingArmorSound = GetComponent<AudioSource>();
    }
    protected override void Interact()
    {
        if (playerArmor.currentArmor <= 99)
        {
            playerArmor.PickupArmor();
            pickingArmorSound.Play();
            Destroy(gameObject);
        }
    }
}
