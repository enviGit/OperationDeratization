using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Player;
using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class FirstAidKit : Interactable
    {
        [Header("References")]
        private PlayerHealth playerHealth;
        private AudioEventManager audioEventManager;
        private AudioSource restoreHealthSound;
        private MeshRenderer mesh;

        [Header("Settings")]
        public float hpToRestore = 15f;
        [SerializeField] private float delayBeforeDestroy = 3.5f;

        private bool used = false;

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) playerHealth = playerObj.GetComponent<PlayerHealth>();

            var audioManagerObj = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioManagerObj) audioEventManager = audioManagerObj.GetComponent<AudioEventManager>();

            restoreHealthSound = GetComponent<AudioSource>();
            mesh = GetComponent<MeshRenderer>();
        }

        protected override void Interact()
        {
            if (used || playerHealth == null) return;

            if (playerHealth.currentHealth < 99f)
            {
                playerHealth.RestoreHealth(hpToRestore);

                prompt = "";
                used = true;
                StartCoroutine(DestroySequence());
            }
        }

        private IEnumerator DestroySequence()
        {
            if (restoreHealthSound)
            {
                restoreHealthSound.Play();
                audioEventManager?.NotifyAudioEvent(restoreHealthSound);
            }

            SetShaderDissolve(0);
            float elapsedTime = 0f;

            while (elapsedTime < delayBeforeDestroy)
            {
                SetShaderDissolve(elapsedTime / delayBeforeDestroy);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
        }

        private void SetShaderDissolve(float value)
        {
            if (mesh) mesh.material.SetFloat("_dissolve", value);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyHealth health = other.GetComponent<EnemyHealth>();
                if (!used && health != null && health.currentHealth <= 99f && health.isAlive)
                {
                    health.RestoreHealth(hpToRestore);
                    used = true;
                    StartCoroutine(DestroySequence());
                }
            }
        }
    }
}