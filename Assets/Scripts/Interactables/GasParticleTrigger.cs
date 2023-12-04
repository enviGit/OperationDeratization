using UnityEngine;

public class GasParticleTrigger : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private bool playerCollision = false;

    private void Update()
    {
        if (!playerCollision)
            CancelInvoke("DealGasDamageToPlayer");
    }
    private void LateUpdate()
    {
        playerCollision = false;
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerCollision = true;
                InvokeRepeating("DealGasDamageToPlayer", 0f, 0.5f);
            }
        }
    }
    private void DealGasDamageToPlayer()
    {
        playerHealth.TakeGasDamage(Random.Range(5, 10));
    }
}