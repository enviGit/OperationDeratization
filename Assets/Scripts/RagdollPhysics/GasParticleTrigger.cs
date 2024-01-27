using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Player;
using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.RagdollPhysics
{
    public class GasParticleTrigger : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        private EnemyHealth enemyHealth;
        private List<GameObject> charactersInTrigger = new List<GameObject>();

        private void Start()
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                charactersInTrigger.Add(other.gameObject);

                if (charactersInTrigger.Count == 1)
                    InvokeRepeating("DealGasDamageOverTime", 0f, 1f);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                charactersInTrigger.Remove(other.gameObject);

                if (charactersInTrigger.Count == 0)
                    CancelInvoke("DealGasDamageOverTime");
            }
        }
        private void DealGasDamageOverTime()
        {
            foreach (var character in charactersInTrigger)
            {
                if (character != null)
                {
                    if (character.CompareTag("Player"))
                    {
                        int damage = Random.Range(10, 20);
                        playerHealth.TakeGasDamage(damage);
                    }
                    else if (character.CompareTag("Enemy"))
                    {
                        enemyHealth = character.GetComponent<EnemyHealth>();

                        if (enemyHealth != null)
                        {
                            int enemyDamage = Random.Range(10, 20);
                            enemyHealth.TakeDamage(enemyDamage, Vector3.zero, false);
                        }
                    }
                }
            }
        }
    }
}