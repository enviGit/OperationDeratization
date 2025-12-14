using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Weapon : Interactable
    {
        [Header("Weapon Data")]
        public Gun gun;
        public RuntimeAnimatorController animator;
        private AiAgent currentHolder = null;

        [Header("References")]
        private PlayerInventory inventory;

        private List<Renderer> availableGrenadeModels = new List<Renderer>();

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) inventory = player.GetComponent<PlayerInventory>();

            if (gun) prompt = $"Pick up {gun.gunName}";

            if (IsGrenadeType())
            {
                var allRenderers = GetComponentsInChildren<Renderer>().ToList();

                var parentRenderer = GetComponent<Renderer>();

                if (parentRenderer != null)
                {
                    allRenderers.Remove(parentRenderer);
                }

                availableGrenadeModels = allRenderers;
            }
        }

        protected override void Interact()
        {
            if (inventory == null || gun == null) return;
            if (IsOwnedByEnemy()) return;

            if (IsGrenadeType())
            {
                HandleSmartGrenadePickup();
            }
            else
            {
                if (inventory.AddItem(gun))
                {
                    Destroy(gameObject);
                }
            }
        }

        private void HandleSmartGrenadePickup()
        {
            if (availableGrenadeModels.Count == 0) return;

            int currentAmmo = 0;
            int slotIdx = (int)gun.gunStyle;

            currentAmmo = inventory.GetGrenadeCount(slotIdx);

            int maxCapacity = gun.maxAmmoCount > 0 ? gun.maxAmmoCount : 3;

            int spaceLeft = maxCapacity - currentAmmo;

            if (spaceLeft <= 0)
            {
                return;
            }

            int amountToTake = Mathf.Min(availableGrenadeModels.Count, spaceLeft);

            var modelsToCheck = new List<Renderer>(availableGrenadeModels);

            for (int i = 0; i < amountToTake; i++)
            {
                Renderer model = modelsToCheck[i];

                bool success = inventory.AddItem(gun);

                if (success)
                {
                    availableGrenadeModels.Remove(model);
                    StartCoroutine(DissolveAndDestroySingle(model));
                }
                else
                {
                    break;
                }
            }

            if (availableGrenadeModels.Count == 0)
            {
                DisableInteraction();
            }
        }

        private IEnumerator DissolveAndDestroySingle(Renderer r)
        {
            if (r == null) yield break;

            float duration = 0.45f;
            float time = 0;

            while (time < duration)
            {
                if (r == null) yield break;
                r.material.SetFloat("_dissolve", time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            if (r != null) Destroy(r.gameObject);
        }

        private void DisableInteraction()
        {
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;

            prompt = "";
        }

        private bool IsOwnedByEnemy()
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                if (parent.CompareTag("Enemy")) return true;
                parent = parent.parent;
            }
            return false;
        }

        private bool IsGrenadeType()
        {
            return gun.gunStyle == GunStyle.Grenade ||
                   gun.gunStyle == GunStyle.Flashbang ||
                   gun.gunStyle == GunStyle.Smoke ||
                   gun.gunStyle == GunStyle.Molotov;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") && currentHolder == null)
            {
                var aiAgent = other.GetComponent<AiAgent>();
                var aiWeapons = other.GetComponent<AiWeapons>();

                if (aiAgent && aiWeapons && aiAgent.stateMachine.currentState != AiStateId.Death)
                {
                    if ((gun.gunStyle == GunStyle.Primary || gun.gunStyle == GunStyle.Secondary) && aiWeapons.currentWeapon == null)
                    {
                        currentHolder = aiAgent;
                        var sockets = other.GetComponentInChildren<MeshSockets>();
                        GameObject newWeapon = Instantiate(gun.gunPrefab);
                        aiWeapons.Equip(newWeapon, sockets);
                        gameObject.tag = "Untagged";
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
                        prompt = "";
                        Destroy(gameObject);
                    }
                }
            }
        }
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}