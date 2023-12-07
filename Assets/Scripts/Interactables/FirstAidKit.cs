using UnityEngine;

public class FirstAidKit : Interactable
{
    [Header("References")]
    public PlayerHealth playerHealth;
    private AudioSource restoreHealthSound;

    private void Start()
    {
        restoreHealthSound = GetComponent<AudioSource>();
    }
    protected override void Interact()
    {
        if (playerHealth.currentHealth < 99f)
        {
            playerHealth.RestoreHealth(Random.Range(15, 25));
            restoreHealthSound.Play();
            Destroy(gameObject);
        }
    }
}