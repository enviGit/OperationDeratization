using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using RatGamesStudios.OperationDeratization.UI.InGame;
using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class EnemyShoot : MonoBehaviour
    {
        [Header("References")]
        public GameObject muzzleFlash;
        public GameObject impactEffect;
        public GameObject impactRicochet;
        public GameObject bloodSpread;
        [SerializeField] private GameObject bloodWound;
        private AudioSource gunFireAudio;
        private AudioSource gunReloadAudio;
        public AiWeapons aiWeapons;
        public Gun currentWeapon;
        public LayerMask layerMask;

        [Header("Weapon")]
        private float autoShotTimer = 0f;
        public bool isReloading = false;
        public bool isFiring = false;
        private bool isLowQuality = false;

        private void Start()
        {
            aiWeapons = GetComponent<AiWeapons>();
            gunFireAudio = transform.Find("Sounds/WeaponFire").GetComponent<AudioSource>();
            gunReloadAudio = transform.Find("Sounds/WeaponReload").GetComponent<AudioSource>();

            if (Settings.QualityPreset == 0)
                isLowQuality = true;
        }
        private void Update()
        {
            if (aiWeapons.currentWeapon != null)
                SetCurrentWeapon();
        }
        public void StartFiring()
        {
            isFiring = true;
        }
        public void StopFiring()
        {
            isFiring = false;
        }
        public void Shoot()
        {
            RaycastHit hit;

            if (currentWeapon != null && currentWeapon.currentAmmoCount == 0 && currentWeapon.maxAmmoCount != 0 && !isReloading)
            {
                StartCoroutine(ReloadCoroutine());

                return;
            }
            if (Time.time > autoShotTimer && currentWeapon != null && currentWeapon.currentAmmoCount > 0 && !isReloading && isFiring)
            {
                gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
                gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                currentWeapon.currentAmmoCount--;
                Transform muzzle = null;
                string[] bonePrefixes = { "mixamorig9:", "mixamorig4:", "mixamorig10:", "mixamorig:" };

                foreach (string prefix in bonePrefixes)
                {
                    muzzle = transform.Find($"{prefix}Hips/{prefix}Spine/{prefix}Spine1/{prefix}Spine2/{prefix}RightShoulder/{prefix}RightArm/{prefix}RightForeArm/{prefix}RightHand/socketRightHand/{currentWeapon.gunPrefab.name}(Clone)/muzzle");

                    if (muzzle != null)
                        break;
                }

                ObjectPoolManager.SpawnObject(muzzleFlash, muzzle.position, muzzle.rotation, muzzle);

                if (Physics.Raycast(muzzle.transform.position, muzzle.forward, out hit, currentWeapon.range, layerMask))
                {
                    Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                    var hitBox = hit.collider.GetComponent<HitBox>();

                    if (currentWeapon.gunType == GunType.Shotgun)
                    {
                        int numPellets = 5;
                        float maxSpread = 0.1f;

                        for (int i = 0; i < numPellets; i++)
                        {
                            Vector3 spreadDirection = muzzle.forward + new Vector3(Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread),
                                Random.Range(-maxSpread, maxSpread));
                            RaycastHit spreadHit;

                            if (Physics.Raycast(muzzle.transform.position, spreadDirection, out spreadHit, currentWeapon.range, layerMask))
                            {
                                ObjectPoolManager.SpawnObject(impactRicochet, spreadHit.point, Quaternion.LookRotation(spreadHit.normal), ObjectPoolManager.PoolType.ParticleSystem);
                                HitBox spreadHitBox = spreadHit.collider.GetComponent<HitBox>();

                                if (!isLowQuality && spreadHitBox == null)
                                {
                                    if (spreadHit.collider.gameObject.GetComponent<Weapon>() == null && !spreadHit.collider.CompareTag("GraveyardWall") && !spreadHit.collider.CompareTag("Glass")
                                        && spreadHit.collider.gameObject.layer != LayerMask.NameToLayer("Water"))
                                    {
                                        if (spreadHit.rigidbody != null || spreadHit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                                            ObjectPoolManager.SpawnObject(impactEffect, spreadHit.point, Quaternion.LookRotation(spreadHit.normal), spreadHit.collider.transform);
                                        else
                                            ObjectPoolManager.SpawnObject(impactEffect, spreadHit.point, Quaternion.LookRotation(spreadHit.normal), ObjectPoolManager.PoolType.ParticleSystem);
                                    }
                                }
                                if (spreadHit.collider.CompareTag("Glass"))
                                    spreadHit.collider.GetComponent<Glass>().Break(spreadHit.point, currentWeapon.impactForce);
                                if (spreadHitBox != null)
                                {
                                    if (spreadHitBox.health != null)
                                    {
                                        spreadHitBox.OnRaycastHit(currentWeapon, muzzle.forward, gameObject); //Or transform.forward
                                        ObjectPoolManager.SpawnObject(bloodSpread, spreadHit.point, Quaternion.LookRotation(spreadHit.normal), spreadHit.collider.transform);

                                        if (!isLowQuality)
                                            ObjectPoolManager.SpawnObject(bloodWound, spreadHit.point, Quaternion.LookRotation(spreadHit.normal), spreadHit.collider.transform);
                                    }
                                    if (spreadHitBox.playerHealth != null)
                                    {
                                        spreadHitBox.OnRaycastHitPlayer(currentWeapon, gameObject);

                                        if (spreadHitBox.damageToPlayer > 0)
                                            DISystem.CreateIndicator(this.transform);
                                    }
                                }
                            }
                            if (spreadHit.rigidbody != null)
                                spreadHit.rigidbody.AddForce(-spreadHit.normal * currentWeapon.impactForce);
                        }
                    }
                    if (hitBox == null)
                    {
                        ObjectPoolManager.SpawnObject(impactRicochet, hit.point, impactRotation, ObjectPoolManager.PoolType.ParticleSystem);

                        if (!isLowQuality)
                        {
                            if (hit.collider.gameObject.GetComponent<Weapon>() == null && !hit.collider.CompareTag("GraveyardWall") && !hit.collider.CompareTag("Glass") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Water"))
                            {
                                if (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                                    ObjectPoolManager.SpawnObject(impactEffect, hit.point, impactRotation, hit.collider.transform);
                                else
                                    ObjectPoolManager.SpawnObject(impactEffect, hit.point, impactRotation, ObjectPoolManager.PoolType.ParticleSystem);
                            }
                        }
                        if (hit.collider.CompareTag("Glass"))
                            hit.collider.GetComponent<Glass>().Break(hit.point, currentWeapon.impactForce);
                    }
                    else
                    {
                        if (hitBox.health != null)
                        {
                            hitBox.OnRaycastHit(currentWeapon, muzzle.forward, gameObject); //Or transform.forward
                            ObjectPoolManager.SpawnObject(bloodSpread, hit.point, impactRotation, hit.collider.transform);

                            if (!isLowQuality)
                                ObjectPoolManager.SpawnObject(bloodWound, hit.point, impactRotation, hit.collider.transform);
                        }
                        if (hitBox.playerHealth != null)
                        {
                            hitBox.OnRaycastHitPlayer(currentWeapon, gameObject);

                            if (hitBox.damageToPlayer > 0)
                                DISystem.CreateIndicator(this.transform);
                        }
                    }
                    if (hit.rigidbody != null)
                        hit.rigidbody.AddForce(-hit.normal * currentWeapon.impactForce);
                }

                autoShotTimer = Time.time + currentWeapon.timeBetweenShots;
            }
        }
        public IEnumerator ReloadCoroutine()
        {
            isReloading = true;
            gunReloadAudio.clip = currentWeapon.gunAudioClips[2];
            gunReloadAudio.Play();

            if (currentWeapon.gunType == GunType.Pistol)
                yield return new WaitForSeconds(2f);
            else if (currentWeapon.gunType == GunType.Revolver || currentWeapon.gunType == GunType.Rifle)
                yield return new WaitForSeconds(3f);
            else if (currentWeapon.gunType == GunType.Shotgun || currentWeapon.gunType == GunType.Sniper)
                yield return new WaitForSeconds(4f);
            if (currentWeapon.currentAmmoCount == currentWeapon.magazineSize)
                yield break;

            int ammoNeeded = currentWeapon.magazineSize - currentWeapon.currentAmmoCount;
            int ammoAvailable = Mathf.Min(currentWeapon.maxAmmoCount, ammoNeeded);

            if (ammoAvailable == 0)
                yield break;

            currentWeapon.currentAmmoCount += ammoAvailable;
            currentWeapon.maxAmmoCount -= currentWeapon.magazineSize;
            isReloading = false;
        }
        private void SetCurrentWeapon()
        {
            currentWeapon = aiWeapons.currentWeapon.GetComponent<Weapon>().gun;
        }
    }
}