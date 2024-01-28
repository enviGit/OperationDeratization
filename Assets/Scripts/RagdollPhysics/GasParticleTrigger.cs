using RatGamesStudios.OperationDeratization.Player;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.RagdollPhysics
{
    public class GasParticleTrigger : MonoBehaviour
    {
        private PlayerHealth playerHealth;

        private void Start()
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                InvokeRepeating("DealGasDamageOverTime", 0f, 1f);
        }
        private void OnTriggerExit(Collider other)
        {
            CancelInvoke("DealGasDamageOverTime");
        }
        private void DealGasDamageOverTime()
        {
            int damage = Random.Range(10, 20);
            playerHealth.TakeGasDamage(damage);
        }
    }
}