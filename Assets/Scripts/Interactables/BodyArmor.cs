using System.Collections;
using UnityEngine;

public class BodyArmor : Interactable
{
    [Header("References")]
    public PlayerHealth playerArmor;
    private AudioSource pickingArmorSound;
    private float delayBeforeDestroy = 1f;
    private bool used = false;

    private void Start()
    {
        pickingArmorSound = GetComponent<AudioSource>();
    }
    protected override void Interact()
    {
        if (!used && playerArmor.currentArmor <= 99)
        {
            playerArmor.PickupArmor();
            StartCoroutine(DestroyAfterSound());
            used = true;
        }
    }
    private IEnumerator DestroyAfterSound()
    {
        pickingArmorSound.Play();

        yield return new WaitForSeconds(delayBeforeDestroy);

        Destroy(gameObject);
    }
}
