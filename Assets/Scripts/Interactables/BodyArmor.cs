using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class BodyArmor : Interactable
    {
        [Header("References")]
        private PlayerHealth playerHealth;
        private AudioEventManager audioEventManager;
        private AudioSource pickingArmorSound;

        [Header("Settings")]
        [SerializeField] private float delayBeforeDestroy = 1f;

        private bool used = false;
        private List<MeshRenderer> meshes = new List<MeshRenderer>();

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) playerHealth = playerObj.GetComponent<PlayerHealth>();

            var audioManagerObj = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioManagerObj) audioEventManager = audioManagerObj.GetComponent<AudioEventManager>();

            pickingArmorSound = GetComponent<AudioSource>();

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out MeshRenderer mr)) meshes.Add(mr);
            }
        }

        protected override void Interact()
        {
            if (used || playerHealth == null) return;

            if (playerHealth.currentArmor <= 99)
            {
                playerHealth.PickupArmor();

                prompt = "";
                used = true;
                StartCoroutine(DestroySequence());
            }
        }

        private IEnumerator DestroySequence()
        {
            if (pickingArmorSound)
            {
                pickingArmorSound.Play();
                audioEventManager?.NotifyAudioEvent(pickingArmorSound);
            }

            SetShaderDissolve(0);
            float elapsedTime = 0f;

            while (elapsedTime < delayBeforeDestroy)
            {
                SetShaderDissolve(elapsedTime / delayBeforeDestroy);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        private void SetShaderDissolve(float value)
        {
            foreach (var mesh in meshes)
            {
                if (mesh) foreach (var mat in mesh.materials) mat.SetFloat("_dissolve", value);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyHealth health = other.GetComponent<EnemyHealth>();

                if (!used && health != null && health.currentArmor <= 99f && health.isAlive)
                {
                    health.PickupArmor();
                    used = true;
                    StartCoroutine(DestroySequence());
                    TryDifferentBonePrefixes(health.transform);
                }
            }
        }

        private void TryDifferentBonePrefixes(Transform character)
        {
            string[] possiblePrefixes = { "mixamorig9:", "mixamorig4:", "mixamorig:", "mixamorig10:" };
            foreach (var prefix in possiblePrefixes)
            {
                Transform armorSocket = character.transform.Find($"{prefix}Hips/{prefix}Spine/{prefix}Spine1/ArmorSocket");
                if (armorSocket != null)
                {
                    if (armorSocket.childCount > 0) armorSocket.GetChild(0).gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
}