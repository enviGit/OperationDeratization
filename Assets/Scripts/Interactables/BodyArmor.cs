using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class BodyArmor : Interactable
    {
        [Header("References")]
        public PlayerHealth playerArmor;
        private AudioSource pickingArmorSound;
        private float delayBeforeDestroy = 1f;
        private bool used = false;
        private List<MeshRenderer> meshes = new List<MeshRenderer>();

        private void Start()
        {
            pickingArmorSound = GetComponent<AudioSource>();

            foreach (Transform child in transform)
            {
                MeshRenderer mr = child.GetComponent<MeshRenderer>();

                if (mr != null)
                    meshes.Add(mr);
            }
        }
        protected override void Interact()
        {
            if (!used && playerArmor.currentArmor <= 99)
            {
                playerArmor.PickupArmor();
                prompt = "";
                StartCoroutine(DestroyAfterSound());
                used = true;
            }
        }
        private IEnumerator DestroyAfterSound()
        {
            pickingArmorSound.Play();
            SetShaderParameters(0);
            float elapsedTime = 0f;
            float duration = delayBeforeDestroy;

            while (elapsedTime < duration)
            {
                SetShaderParameters(elapsedTime / duration);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }
        private void TryDifferentBonePrefixes(Transform character)
        {
            string[] possiblePrefixes = { "mixamorig9:", "mixamorig4:", "mixamorig:", "mixamorig10:" };

            foreach (var prefix in possiblePrefixes)
            {
                Transform armorSocket = character.transform.Find($"{prefix}Hips/{prefix}Spine/{prefix}Spine1/ArmorSocket");

                if (armorSocket != null)
                {
                    armorSocket.GetChild(0).gameObject.SetActive(true);

                    break;
                }
            }
        }
        private void SetShaderParameters(float disappearIntensity)
        {
            foreach (MeshRenderer meshRenderer in meshes)
            {
                Material[] materials = meshRenderer.materials;

                foreach (var material in materials)
                    material.SetFloat("_dissolve", disappearIntensity);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyHealth health = other.GetComponent<EnemyHealth>();

                if (!used && health.currentArmor <= 99f && health.isAlive)
                {
                    health.PickupArmor();
                    StartCoroutine(DestroyAfterSound());
                    used = true;
                    TryDifferentBonePrefixes(health.transform);
                }
            }
        }
    }
}