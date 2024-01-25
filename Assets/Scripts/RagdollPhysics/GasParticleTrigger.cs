using RatGamesStudios.OperationDeratization.Player;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.RagdollPhysics
{
    public class GasParticleTrigger : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        private bool playerCollision = false;
        [SerializeField] private bool isTutorialActive = false;

        private void Start()
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }
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
                    playerCollision = true;

                    if(isTutorialActive)
                        InvokeRepeating("DealGasDamageToPlayer", 0f, 1f);
                    else
                        InvokeRepeating("DealGasDamageToPlayer", 0f, 0.5f);
            }
        }
        private void DealGasDamageToPlayer()
        {
            int damage = isTutorialActive ? Random.Range(1, 2) : Random.Range(5, 10);
            playerHealth.TakeGasDamage(damage);
        }
    }
}