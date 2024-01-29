using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RatGamesStudios.OperationDeratization.RagdollPhysics
{
    public class FireParticleTrigger : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        private Dictionary<GameObject, VisualEffect> characterVfxMap = new Dictionary<GameObject, VisualEffect>();
        private bool isCoroutineRunning = false;

        private void Start()
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                characterVfxMap[other.gameObject] = other.GetComponent<VisualEffect>();

                if (!isCoroutineRunning && characterVfxMap.Count > 0)
                {
                    StartCoroutine(DealFireDamageOverTimeCoroutine());
                    isCoroutineRunning = true;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (characterVfxMap.ContainsKey(other.gameObject))
                {
                    var vfxComponent = characterVfxMap[other.gameObject];
                    characterVfxMap.Remove(other.gameObject);

                    if (characterVfxMap.Count == 0)
                    {
                        StopCoroutine("DealFireDamageOverTimeCoroutine");
                        isCoroutineRunning = false;

                        if (vfxComponent != null)
                            vfxComponent.SendEvent(VisualEffectAsset.StopEventName);
                    }
                }
            }
        }
        private IEnumerator DealFireDamageOverTimeCoroutine()
        {
            while (characterVfxMap.Count > 0)
            {
                yield return new WaitForSeconds(1f);

                foreach (var pair in characterVfxMap)
                {
                    var character = pair.Key;
                    var isVfxActive = pair.Value;

                    if (isVfxActive)
                    {
                        if (character.CompareTag("Player"))
                        {
                            int damage = Random.Range(5, 10);
                            playerHealth.TakeFireDamage(damage);
                        }
                        else if (character.CompareTag("Enemy"))
                        {
                            int enemyDamage = Random.Range(5, 10);
                            character.GetComponent<EnemyHealth>().TakeDamage(enemyDamage, Vector3.zero, false);

                            var vfxComponent = character.GetComponent<VisualEffect>();

                            if (vfxComponent != null)
                                vfxComponent.SendEvent(VisualEffectAsset.PlayEventName);
                        }
                    }
                }
            }
        }
        private void OnDisable()
        {
            StopCoroutine("DealFireDamageOverTimeCoroutine");
            isCoroutineRunning = false;

            foreach (var vfxComponent in characterVfxMap.Values)
            {
                if (vfxComponent != null)
                    vfxComponent.SendEvent(VisualEffectAsset.StopEventName);
            }

            characterVfxMap.Clear();
        }
    }
}