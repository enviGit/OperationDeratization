using System.Collections;
using UnityEngine;

public class FirstAidKit : Interactable
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public float hpToRestore = 15f;
    private float delayBeforeDestroy = 3.5f;
    private AudioSource restoreHealthSound;
    private bool used = false;

    private void Start()
    {
        restoreHealthSound = GetComponent<AudioSource>();
    }
    protected override void Interact()
    {
        if (!used && playerHealth.currentHealth < 99f)
        {
            playerHealth.RestoreHealth(hpToRestore);
            StartCoroutine(DestroyAfterSound());
            used = true;
        }
    }
    private IEnumerator DestroyAfterSound()
    {
        restoreHealthSound.Play();

        yield return new WaitForSeconds(delayBeforeDestroy);

        Destroy(gameObject);
    }
}