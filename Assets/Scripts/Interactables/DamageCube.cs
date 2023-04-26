using UnityEngine;

public class DamageCube : Interactable
{
    [Header("References")]
    private PlayerHealth hp;

    private void Start()
    {
        hp = FindObjectOfType<PlayerHealth>();
    }
    protected override void Interact()
    {
        if (hp != null)
            hp.TakeDamage(Random.Range(5, 20));
    }
}