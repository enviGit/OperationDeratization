using RatGamesStudios.OperationDeratization.Equipment;
using RatGamesStudios.OperationDeratization.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class AiWeapons : MonoBehaviour
    {
        [Header("References")]
        [HideInInspector] public GameObject currentWeapon;
        private EnemyShoot weapon;
        public bool hasLootedAmmo = false;
        public ActiveWeapon ActiveGunLogic => activeGunLogic;
        private ActiveWeapon activeGunLogic;

        [Header("Weapons")]
        private Animator animator;
        private MeshSockets weaponSockets;
        private WeaponIk weaponIk;
        public Transform currentTarget;
        public bool weaponActive = false;
        public float inaccuracy = 0.4f;

        private void Start()
        {
            animator = GetComponent<Animator>();
            weaponIk = GetComponent<WeaponIk>();
            weapon = GetComponent<EnemyShoot>();
        }
        private void Update()
        {
            if (currentTarget && currentWeapon && weaponActive)
            {
                Vector3 target = currentTarget.position;
                target += Random.insideUnitSphere * inaccuracy;
                weapon.Shoot();
            }
        }
        public void SetFiring(bool enabled)
        {
            if (enabled)
                weapon.StartFiring();
            else
                weapon.StopFiring();
        }
        public void Equip(GameObject weaponObj, MeshSockets sockets)
        {
            weaponSockets = sockets;
            currentWeapon = weaponObj;

            var wScript = currentWeapon.GetComponent<Weapon>();
            if (wScript)
            {
                wScript.prompt = "";
                wScript.enabled = false;
            }

            currentWeapon.tag = "Untagged";
            currentWeapon.layer = LayerMask.NameToLayer("Default");
            SetLayerRecursively(currentWeapon, LayerMask.NameToLayer("Default"));

            sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.Spine);

            activeGunLogic = currentWeapon.GetComponentInChildren<ActiveWeapon>();

            if (activeGunLogic != null)
            {
                activeGunLogic.InitializeAmmo();
            }
        }
        public void ActiveWeapon()
        {
            StartCoroutine(EquipWeapon());
        }
        private IEnumerator EquipWeapon()
        {
            var wScript = currentWeapon.GetComponent<Weapon>();
            if (wScript && wScript.animator)
            {
                animator.runtimeAnimatorController = wScript.animator;
            }
            animator.SetBool("Equip", true);

            yield return new WaitForSeconds(0.5f);

            while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
                yield return null;

            if (currentWeapon != null && currentWeapon.transform != null)
            {
                Transform muzzleTransform = currentWeapon.transform.Find("muzzle");

                if (muzzleTransform == null && wScript && wScript.gun)
                {
                }

                if (muzzleTransform != null)
                {
                    weaponIk.SetAimTransform(muzzleTransform);
                    weaponActive = true;
                }
                else
                {
                    weaponIk.SetAimTransform(currentWeapon.transform);
                    weaponActive = true;
                }
            }
        }
        public void DeactiveWeapon()
        {
            SetTarget(null);
            SetFiring(false);
            StartCoroutine(HolsterWeapon());
        }
        private IEnumerator HolsterWeapon()
        {
            weaponActive = false;
            //animator.SetBool("Equip", false);

            //yield return new WaitForSeconds(0.5f);

            while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
                yield return null;

            if (currentWeapon != null)
                weaponIk.SetAimTransform(currentWeapon.transform.Find("muzzle"));
        }
        public void DropWeapon()
        {
            if (currentWeapon)
            {
                currentWeapon.transform.SetParent(null);

                Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
                if (!rb) rb = currentWeapon.AddComponent<Rigidbody>();

                rb.isKinematic = false;
                rb.useGravity = true;
                rb.mass = 2f;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                var wScript = currentWeapon.GetComponent<Weapon>();
                if (wScript) wScript.enabled = true;

                currentWeapon.tag = "Weapon";
                currentWeapon.layer = LayerMask.NameToLayer("Interactable");
                SetLayerRecursively(currentWeapon, LayerMask.NameToLayer("Interactable"));

                currentWeapon = null;
                activeGunLogic = null;
            }
        }
        public bool HasWeapon()
        {
            return currentWeapon != null;
        }
        public void OnAnimationEvent(string eventName)
        {
            if (eventName == "equipWeapon" && currentWeapon != null)
            {
                weaponSockets.Attach(currentWeapon.transform, MeshSockets.SocketId.RightHand);

                var wScript = currentWeapon.GetComponent<Weapon>();
                if (wScript && wScript.gun)
                {
                    GunType gunType = wScript.gun.gunType;

                    if (gunType == GunType.Pistol)
                        currentWeapon.transform.localPosition = new Vector3(0.0836f, -0.0644f, -0.0415f);
                    else if (gunType == GunType.Revolver)
                        currentWeapon.transform.localPosition = new Vector3(0.1347f, -0.0921f, -0.1241f);
                    else if (gunType == GunType.Sniper)
                        currentWeapon.transform.localPosition = new Vector3(0.1012f, 0.039f, 0.196f);
                    else
                        currentWeapon.transform.localPosition = Vector3.zero;
                }
            }
        }
        public void SetTarget(Transform target)
        {
            //weaponIk.SetTargetTransform(target);
            //currentTarget = target;

            if (target != null)
            {
                Collider[] hitboxes = target.GetComponentsInChildren<Collider>();

                if (hitboxes.Length > 0)
                {
                    List<Collider> hitboxesWithHitboxLayer = new List<Collider>();

                    foreach (Collider collider in hitboxes)
                    {
                        if (collider.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
                            hitboxesWithHitboxLayer.Add(collider);
                    }

                    if (hitboxesWithHitboxLayer.Count > 0)
                    {
                        List<string> preferredParts = new List<string> { "head", "spine" };
                        List<Collider> preferredHitboxes = new List<Collider>();

                        foreach (string part in preferredParts)
                            preferredHitboxes.AddRange(hitboxesWithHitboxLayer.FindAll(hitbox => hitbox.name.ToLower().Contains(part)));

                        Collider chosenHitbox;

                        if (preferredHitboxes.Count > 0)
                            chosenHitbox = preferredHitboxes[Random.Range(0, preferredHitboxes.Count)];
                        else
                            chosenHitbox = hitboxesWithHitboxLayer[Random.Range(0, hitboxesWithHitboxLayer.Count)];

                        weaponIk.SetTargetTransform(chosenHitbox.transform);
                        currentTarget = chosenHitbox.transform;
                    }
                }
            }
            else
            {
                weaponIk.SetTargetTransform(target);
                currentTarget = target;
            }
        }
        public void RefillAmmo(int magazineSize)
        {
            if (activeGunLogic == null) return;

            if (!activeGunLogic.IsFull())
            {
                activeGunLogic.AddAmmo(magazineSize);
                hasLootedAmmo = true;
            }
        }
        public bool IsLowAmmo()
        {
            if (activeGunLogic == null) return true;

            return activeGunLogic.CurrentClip <= 0 && activeGunLogic.CurrentStash <= 0;
        }
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}