using UnityEngine;
using System.Collections.Generic;

public class GasParticleTrigger : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
                InvokeRepeating("DealGasDamage", 0f, 0.5f);
        }
    }
    private void DealGasDamage()
    {
        if (playerHealth != null)
            playerHealth.TakeGasDamage(Random.Range(3, 15));
    }
}