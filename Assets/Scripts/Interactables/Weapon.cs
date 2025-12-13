using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Player;
using RatGamesStudios.OperationDeratization.Enemy.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Weapon : Interactable
    {
        [Header("Weapon Data")]
        public Gun gun;
        public RuntimeAnimatorController animator;

        [Header("References")]
        private PlayerInventory inventory;
        
        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) inventory = player.GetComponent<PlayerInventory>();
            
            if (gun) prompt = $"Pick up {gun.gunName}";
        }

        protected override void Interact()
        {
            if (inventory == null || gun == null) return;

            if (IsOwnedByEnemy()) return;

            if (IsGrenadeType())
            {
                HandleGrenadePickup();
            }
            else if (gun.gunStyle == GunStyle.Primary || gun.gunStyle == GunStyle.Secondary)
            {
                PickupWeapon();
                Destroy(gameObject);
            }
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

        private void HandleGrenadePickup()
        {
            inventory.AddItem(gun);
            
            if (transform.childCount > 0)
            {
                StartCoroutine(DestroyAfterPickup(transform.GetChild(0).GetComponent<MeshRenderer>()));
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void PickupWeapon()
        {
            inventory.AddItem(gun);
            
            Transform weaponHolder = Camera.main.transform.Find("WeaponHolder");
            if (weaponHolder)
            {
                GameObject weaponObject = Instantiate(gun.gunPrefab, weaponHolder);
                weaponObject.layer = LayerMask.NameToLayer("Player");
                
                Destroy(weaponObject.GetComponent<Weapon>());
                
                weaponObject.transform.localPosition = Vector3.zero;
                weaponObject.transform.localRotation = Quaternion.identity;
                
                DisableShadows(weaponObject.transform);
                
                int childIndex = (int)gun.gunStyle;
                weaponObject.transform.SetSiblingIndex(childIndex);
            }
        }

        private void DisableShadows(Transform parent)
        {
            foreach (var r in parent.GetComponentsInChildren<Renderer>())
            {
                r.shadowCastingMode = ShadowCastingMode.Off;
                r.receiveShadows = false;
            }
        }

        private IEnumerator DestroyAfterPickup(MeshRenderer mesh)
        {
            if (mesh == null) yield break;
            
            float duration = 0.45f;
            float time = 0;
            
            while (time < duration)
            {
                if (mesh) mesh.material.SetFloat("_dissolve", time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            
            if (mesh) Destroy(mesh.gameObject);
            
            if (transform.childCount == 0) Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                var aiAgent = other.GetComponent<AiAgent>();
                var aiWeapons = other.GetComponent<AiWeapons>();
                
                if (aiAgent && aiWeapons && aiAgent.stateMachine.currentState != AiStateId.Death)
                {
                    if ((gun.gunStyle == GunStyle.Primary || gun.gunStyle == GunStyle.Secondary) && aiWeapons.currentWeapon == null)
                    {
                        var sockets = other.GetComponentInChildren<MeshSockets>();
                        GameObject newWeapon = Instantiate(gun.gunPrefab);
                        aiWeapons.Equip(newWeapon, sockets);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}