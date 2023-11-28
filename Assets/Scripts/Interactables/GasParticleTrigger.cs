using UnityEngine;
using System.Collections.Generic;

public class GasParticleTrigger : MonoBehaviour
{
    private bool playerInside = false;
    private PlayerHealth playerHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            playerInside = true;
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                InvokeRepeating("DealGasDamage", 0f, 0.5f);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger");
            playerInside = false;
            CancelInvoke("DealGasDamage");
        }
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> enterParticles = new List<ParticleSystem.Particle>();
        int numEnterParticles = GetComponent<ParticleSystem>().GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterParticles);

        Debug.Log("Particle trigger event. Num particles: " + numEnterParticles);

        for (int i = 0; i < numEnterParticles; i++)
        {
            // Do something when a particle enters the trigger zone
            if (playerInside)
            {
                Debug.Log("Dealing gas damage");
                DealGasDamage();
            }
        }
    }

    void DealGasDamage()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeGasDamage(Random.Range(11, 24));
        }
    }
}
