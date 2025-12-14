using RatGamesStudios.OperationDeratization.Equipment;
using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Optimization;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using RatGamesStudios.OperationDeratization.UI;
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
        private ActiveWeapon activeGunLogic;
        public LayerMask layerMask;
        private LayerMask selfHitboxLayer;
        private AudioEventManager audioEventManager;

        [Header("Weapon")]
        private float autoShotTimer = 0f;
        public bool isReloading = false;
        public bool isFiring = false;
        private bool isLowQuality = false;

        [Header("Spread & Burst Settings")]
        public int burstCount = 3;
        public float burstDelay = 0.08f;
        public float resetTime = 0.5f;
        private float maxSpreadAngle = 1.0f;
        private float distancePenaltyMultiplier = 0.01f;

        [HideInInspector] public int currentBurstShots = 0;
        private float lastShotTimeInBurst = 0f;
        private float lastShotTime = 0f;
        private float currentSpread = 0f;

        private void Start()
        {
            aiWeapons = GetComponent<AiWeapons>();

            Transform fireSound = transform.Find("Sounds/WeaponFire");
            if (fireSound) gunFireAudio = fireSound.GetComponent<AudioSource>();

            Transform reloadSound = transform.Find("Sounds/WeaponReload");
            if (reloadSound) gunReloadAudio = reloadSound.GetComponent<AudioSource>();

            var audioMgr = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioMgr) audioEventManager = audioMgr.GetComponent<AudioEventManager>();

            if (Settings.QualityPreset == 0)
                isLowQuality = true;

            selfHitboxLayer = 1 << gameObject.layer;
            int hitboxLayerIndex = LayerMask.NameToLayer("Hitbox");

            if (hitboxLayerIndex != -1)
            {
                layerMask = layerMask & ~hitboxLayerIndex;
            }
        }
        private void Update()
        {
            if (aiWeapons.currentWeapon != null)
            {
                if (activeGunLogic == null || activeGunLogic.gameObject != aiWeapons.currentWeapon)
                {
                    SetCurrentWeapon();
                }
            }
            else
            {
                currentWeapon = null;
                activeGunLogic = null;
            }
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
            if (currentWeapon == null || activeGunLogic == null) return;

            if (activeGunLogic.CurrentClip <= 0 && activeGunLogic.CurrentStash > 0 && !isReloading)
            {
                StartCoroutine(ReloadCoroutine());
                return;
            }

            if (Time.time - lastShotTime > resetTime)
            {
                currentSpread = 0f;
            }


            if (isFiring && !isReloading)
            {
                if (currentBurstShots <= 0)
                {
                    if (Time.time > lastShotTime + currentWeapon.timeBetweenShots * 4f)
                    {
                        currentBurstShots = burstCount;
                    }
                    else
                    {
                        return;
                    }
                }

                if (currentBurstShots > 0)
                {
                    if (Time.time > lastShotTimeInBurst)
                    {
                        if (!activeGunLogic.TryShoot())
                        {
                            currentBurstShots = 0;
                            return;
                        }


                        Transform muzzle = FindMuzzle();
                        if (muzzle == null) return;

                        currentSpread += maxSpreadAngle / burstCount;

                        float distanceToTarget = 50f;
                        if (aiWeapons.currentTarget != null)
                        {
                            distanceToTarget = Vector3.Distance(muzzle.position, aiWeapons.currentTarget.position);
                        }

                        float finalSpread = currentSpread + (distanceToTarget * distancePenaltyMultiplier);
                        finalSpread = Mathf.Clamp(finalSpread, 0f, maxSpreadAngle * 2f);

                        Vector3 originalDirection = muzzle.forward;
                        Vector3 randomSpread = Random.insideUnitSphere * finalSpread;
                        Vector3 finalDirection = (originalDirection + randomSpread).normalized;

                        if (gunFireAudio)
                        {
                            gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
                            if (currentWeapon.gunAudioClips.Length > 0)
                                gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                            if (audioEventManager) audioEventManager.NotifyAudioEvent(gunFireAudio);
                        }

                        ObjectPoolManager.SpawnObject(muzzleFlash, muzzle.position, muzzle.rotation, muzzle);

                        if (Physics.Raycast(muzzle.transform.position, finalDirection, out RaycastHit hit, currentWeapon.range, layerMask))
                        {
                            ProcessHit(hit, finalDirection);
                        }

                        currentBurstShots--;
                        lastShotTimeInBurst = Time.time + burstDelay;
                        lastShotTime = Time.time;
                    }
                }
            }
        }

        public Transform FindMuzzle()
        {
            string[] bonePrefixes = { "mixamorig:", "mixamorig1:", "mixamorig4:", "mixamorig6:", "mixamorig7:", "mixamorig9:", "mixamorig10:", "mixamorig12:" };

            foreach (string prefix in bonePrefixes)
            {
                Transform t = transform.Find($"{prefix}Hips/{prefix}Spine/{prefix}Spine1/{prefix}Spine2/{prefix}RightShoulder/{prefix}RightArm/{prefix}RightForeArm/{prefix}RightHand/socketRightHand/{currentWeapon.gunPrefab.name}(Clone)/muzzle");
                if (t != null) return t;
            }
            return null;
        }

        private void ProcessHit(RaycastHit hit, Vector3 direction)
        {
            Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
            var hitBox = hit.collider.GetComponent<HitBox>();

            if (currentWeapon.gunType == GunType.Shotgun)
            {
                int numPellets = 5;
                float maxSpread = 0.1f;
                for (int i = 0; i < numPellets; i++)
                {
                    Vector3 spreadDir = direction + new Vector3(Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread));
                    if (Physics.Raycast(hit.point - direction * 0.1f, spreadDir, out RaycastHit spreadHit, currentWeapon.range, layerMask))
                    {
                        HandleBulletImpact(spreadHit, spreadDir, impactRotation);
                    }
                }
            }
            else
            {
                HandleBulletImpact(hit, direction, impactRotation);
            }
        }

        private void HandleBulletImpact(RaycastHit hit, Vector3 direction, Quaternion rotation)
        {
            var hitBox = hit.collider.GetComponent<HitBox>();

            if (hitBox == null)
            {
                ObjectPoolManager.SpawnObject(impactRicochet, hit.point, rotation, ObjectPoolManager.PoolType.ParticleSystem);

                if (!isLowQuality)
                {
                    bool isIgnored = hit.collider.GetComponent<Weapon>() != null ||
                                     hit.collider.CompareTag("GraveyardWall") ||
                                     hit.collider.CompareTag("Glass") ||
                                     hit.collider.gameObject.layer == LayerMask.NameToLayer("Water");

                    if (!isIgnored)
                    {
                        Transform parent = (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable")) ? hit.collider.transform : null;
                        ObjectPoolManager.PoolType type = parent ? ObjectPoolManager.PoolType.VFX : ObjectPoolManager.PoolType.ParticleSystem;
                        ObjectPoolManager.SpawnObject(impactEffect, hit.point, rotation, parent);
                    }
                }

                if (hit.collider.CompareTag("Glass"))
                    hit.collider.GetComponent<Glass>()?.Break(hit.point, currentWeapon.impactForce);
            }
            else
            {
                if (hitBox.health != null)
                {
                    hitBox.OnRaycastHit(currentWeapon, direction, gameObject);
                    ObjectPoolManager.SpawnObject(bloodSpread, hit.point, rotation, ObjectPoolManager.PoolType.VFX);
                    if (!isLowQuality)
                        ObjectPoolManager.SpawnObject(bloodWound, hit.point, rotation, hit.collider.transform);
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

        public IEnumerator ReloadCoroutine()
        {
            isReloading = true;
            if (currentWeapon.gunAudioClips.Length > 2 && gunReloadAudio)
            {
                gunReloadAudio.clip = currentWeapon.gunAudioClips[2];
                gunReloadAudio.Play();
                if (audioEventManager) audioEventManager.NotifyAudioEvent(gunReloadAudio);
            }

            yield return new WaitForSeconds(currentWeapon.reloadTime);

            if (activeGunLogic != null)
            {
                activeGunLogic.Reload();
            }

            isReloading = false;
        }
        private void SetCurrentWeapon()
        {
            if (aiWeapons.currentWeapon == null) return;

            var wScript = aiWeapons.currentWeapon.GetComponent<Weapon>();
            if (wScript)
            {
                currentWeapon = wScript.gun;
            }
            else
            {
                Debug.LogWarning($"Bot {name} ma broń bez komponentu Weapon!");
                return;
            }

            activeGunLogic = aiWeapons.currentWeapon.GetComponentInChildren<ActiveWeapon>();

            if (activeGunLogic == null)
            {

            }
            else
            {
                activeGunLogic.InitializeAmmo();
            }
        }
    }
}