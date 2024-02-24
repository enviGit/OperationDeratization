using RatGamesStudios.OperationDeratization.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class AiWeapons : MonoBehaviour
    {
        /*public enum WeaponState
        {
            Holstering,
            Holstered,
            Activating,
            Active,
            Reloading
        }
        public enum WeaponSlot
        {
            Primary,
            Secondary
        }
        public Gun currentGun
        {
            get
            {
                return weapons[current];
            }
        }
        public WeaponSlot currentWeaponSlot
        {
            get
            {
                return (WeaponSlot)current;
            }
        }
        private Gun[] weapons = new Gun[2];
        private int current = 0;
        private WeaponState weaponState = WeaponState.Holstered
        private GameObject magazineHand
        public bool IsActive()
        {
            return weaponState == WeaponState.Active;
        }*/

        [Header("References")]
        [HideInInspector] public GameObject currentWeapon;
        private EnemyShoot weapon;
        public bool hasLootedAmmo = false;

        [Header("Weapons")]
        private Animator animator;
        private MeshSockets weaponSockets;
        private WeaponIk weaponIk;
        private Transform currentTarget;
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
        public void Equip(GameObject weapon, MeshSockets sockets)
        {
            weaponSockets = sockets;
            currentWeapon = weapon;
            currentWeapon.GetComponent<Weapon>().prompt = "";
            currentWeapon.GetComponent<Weapon>().enabled = false;
            currentWeapon.tag = "Untagged";
            currentWeapon.layer = LayerMask.NameToLayer("Default");
            SetLayerRecursively(currentWeapon, LayerMask.NameToLayer("Default"));
            sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.Spine);
        }
        public void ActiveWeapon()
        {
            StartCoroutine(EquipWeapon());
        }
        private IEnumerator EquipWeapon()
        {
            animator.runtimeAnimatorController = currentWeapon.GetComponent<Weapon>().animator;
            animator.SetBool("Equip", true);

            yield return new WaitForSeconds(0.5f);

            while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
                yield return null;

            if (currentWeapon != null && currentWeapon.transform != null)
            {
                Transform muzzleTransform = currentWeapon.transform.Find("muzzle");

                if (muzzleTransform != null)
                {
                    weaponIk.SetAimTransform(muzzleTransform);
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
                Rigidbody rb = currentWeapon.AddComponent<Rigidbody>();
                rb.mass = 2f;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                currentWeapon.GetComponent<Weapon>().enabled = true;
                currentWeapon.tag = "Weapon";
                currentWeapon.layer = LayerMask.NameToLayer("Interactable");
                SetLayerRecursively(currentWeapon, LayerMask.NameToLayer("Interactable"));
                currentWeapon = null;
            }
        }
        public bool HasWeapon()
        {
            return currentWeapon != null;
        }
        public void OnAnimationEvent(string eventName)
        {
            if (eventName == "equipWeapon")
            {
                weaponSockets.Attach(currentWeapon.transform, MeshSockets.SocketId.RightHand);

                if (currentWeapon)
                {
                    GunType gunType = currentWeapon.GetComponent<Weapon>().gun.gunType;

                    if (gunType == GunType.Pistol)
                        currentWeapon.transform.localPosition = new Vector3(0.0836f, -0.0644f, -0.0415f);
                    else if (gunType == GunType.Revolver)
                        currentWeapon.transform.localPosition = new Vector3(0.1347f, -0.0921f, -0.1241f);
                    else if(gunType == GunType.Sniper)
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
            Gun weapon = currentWeapon.GetComponent<Weapon>().gun;

            if (weapon && weapon.maxAmmoCount < weapon.editorAmmoValue * 3)
            {
                weapon.maxAmmoCount += magazineSize;
                hasLootedAmmo = true;
            }
        }
        public bool IsLowAmmo()
        {
            Gun weapon = currentWeapon.GetComponent<Weapon>().gun;

            if (weapon)
                return weapon.currentAmmoCount == 0 && weapon.maxAmmoCount == 0;

            return false;
        }
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}