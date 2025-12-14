using RatGamesStudios.OperationDeratization.Equipment;
using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Optimization;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponRecoil recoil;
        [SerializeField] private WeaponSway sway;

        [Header("Visuals & Audio")]
        public GameObject muzzleFlash;
        public GameObject impactEffect;
        public GameObject impactRicochet;
        public GameObject bloodSpread;
        [SerializeField] private GameObject bloodWound;

        private AudioSource gunFireAudio;
        private AudioSource gunReloadAudio;
        private AudioSource gunSwitchAudio;
        private AudioEventManager audioEventManager;

        [Header("State")]
        public bool isAiming = false;
        public bool _isClimbing = false;
        private bool isReloading = false;
        private bool isLowQuality = false;

        [Header("Runtime")]
        private Gun currentWeapon;
        private GameObject currentWeaponModel;

        private ActiveWeapon activeGunLogic;

        private Animator weaponAnimator;
        private Camera cam;
        public Camera sniperCam;

        private float nextShotTime = 0f;
        private float dynamicFieldOfView = 25f;

        private PlayerMotor playerMotor;
        private float xRotation = 0f;
        public float sensitivity = 3f;

        private void Start()
        {
            cam = Camera.main;
            playerMotor = GetComponent<PlayerMotor>();

            var audioMgr = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioMgr) audioEventManager = audioMgr.GetComponent<AudioEventManager>();

            Transform sounds = transform.Find("Sounds");
            if (sounds)
            {
                gunFireAudio = sounds.Find("WeaponFire")?.GetComponent<AudioSource>();
                gunReloadAudio = sounds.Find("WeaponReload")?.GetComponent<AudioSource>();
                gunSwitchAudio = sounds.Find("WeaponSwitch")?.GetComponent<AudioSource>();
            }

            sensitivity *= Settings.Sensitivity;
            if (Settings.QualityPreset == 0) isLowQuality = true;
        }

        public void OnWeaponChanged(Gun newGun, GameObject newModel)
        {
            Gun previousWeapon = currentWeapon;
            currentWeapon = newGun;
            currentWeaponModel = newModel;

            if (currentWeaponModel != null)
            {
                activeGunLogic = currentWeaponModel.GetComponentInChildren<ActiveWeapon>();
                weaponAnimator = currentWeaponModel.GetComponent<Animator>();
            }
            else
            {
                activeGunLogic = null;
                weaponAnimator = null;
            }

            isReloading = false;
            isAiming = false;
            if (sniperCam) sniperCam.gameObject.SetActive(false);

            if (previousWeapon != null && previousWeapon != currentWeapon && gunSwitchAudio)
            {
                if (Time.time > 0.1f)
                {
                    gunSwitchAudio.pitch = Random.Range(0.85f, 1.15f);
                    int clipIndex = (currentWeapon.gunStyle == GunStyle.Melee) ? 0 : 3;

                    if (currentWeapon.gunAudioClips.Length > clipIndex)
                        gunSwitchAudio.PlayOneShot(currentWeapon.gunAudioClips[clipIndex]);
                }
            }
        }

        private void Update()
        {
            if (playerMotor) _isClimbing = playerMotor._isClimbing;
            if (currentWeapon == null) return;

            HandleAimingAndLook();

            if (IsGrenade(currentWeapon.gunStyle))
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, Time.deltaTime * 5f);
                return;
            }

            HandleInput();
            HandleReload();
        }

        private void HandleInput()
        {
            int currentAmmo = activeGunLogic != null ? activeGunLogic.CurrentClip : 0;
            bool isMelee = currentWeapon.gunStyle == GunStyle.Melee;

            if (Input.GetMouseButton(0) && (currentAmmo > 0 || isMelee) && !isReloading)
            {
                if (currentWeapon.autoFire)
                {
                    if (Time.time >= nextShotTime) Shoot();
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) && Time.time >= nextShotTime) Shoot();
                }
            }
            else if (Input.GetMouseButtonDown(0) && currentAmmo <= 0 && !isReloading && !isMelee)
            {
                if (Time.time >= nextShotTime)
                {
                    gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
                    if (currentWeapon.gunAudioClips.Length > 1)
                        gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[1]);
                    nextShotTime = Time.time + 0.2f;
                }
            }
        }

        private void Shoot()
        {
            if (currentWeapon.gunStyle == GunStyle.Melee)
            {
                if (weaponAnimator) weaponAnimator.SetTrigger("Attack");
                nextShotTime = Time.time + currentWeapon.timeBetweenShots;
                gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                PerformRaycastShot();
                return;
            }

            if (activeGunLogic != null)
            {
                bool shotSuccess = activeGunLogic.TryShoot();
                if (!shotSuccess) return;
            }

            nextShotTime = Time.time + currentWeapon.timeBetweenShots;

            if (recoil) recoil.RecoilFire();

            if (currentWeaponModel)
            {
                Transform muzzle = currentWeaponModel.transform.Find("muzzle");
                if (muzzle) ObjectPoolManager.SpawnObject(muzzleFlash, muzzle.position, muzzle.rotation, muzzle);
            }

            gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
            gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
            audioEventManager?.NotifyAudioEvent(gunFireAudio);

            if (currentWeapon.gunType == GunType.Shotgun)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector3 spread = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                    PerformRaycastShot(spread);
                }
            }
            else
            {
                PerformRaycastShot();
            }
        }

        private void PerformRaycastShot(Vector3 spreadOffset = default)
        {
            Vector3 direction = cam.transform.forward + spreadOffset;
            LayerMask mask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Postprocessing"));

            if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit, currentWeapon.range, mask))
            {
                HandleHit(hit, direction);
            }
        }

        private void HandleHit(RaycastHit hit, Vector3 direction)
        {
            Quaternion rot = Quaternion.LookRotation(hit.normal);
            var hitBox = hit.collider.GetComponent<HitBox>();

            if (hitBox != null)
            {
                if (hitBox.playerHealth == null)
                {
                    hitBox.OnRaycastHit(currentWeapon, direction, gameObject);
                    ObjectPoolManager.SpawnObject(bloodSpread, hit.point, rot, ObjectPoolManager.PoolType.VFX);
                    if (!isLowQuality) ObjectPoolManager.SpawnObject(bloodWound, hit.point, rot, hit.collider.transform);
                }
            }
            else
            {
                if (currentWeapon.gunStyle != GunStyle.Melee)
                {
                    if (!hit.collider.CompareTag("Glass"))
                    {
                        ObjectPoolManager.SpawnObject(impactRicochet, hit.point, rot, ObjectPoolManager.PoolType.ParticleSystem);
                        if (!isLowQuality) ObjectPoolManager.SpawnObject(impactEffect, hit.point, rot, hit.collider.transform);
                    }
                    else
                    {
                        hit.collider.GetComponent<Glass>()?.Break(hit.point, currentWeapon.impactForce);
                    }
                }
                else
                {
                    if (hit.collider.CompareTag("Glass"))
                    {
                        hit.collider.GetComponent<Glass>()?.Break(hit.point, currentWeapon.impactForce);
                    }
                }
            }
            if (hit.rigidbody) hit.rigidbody.AddForceAtPosition(direction.normalized * currentWeapon.impactForce, hit.point, ForceMode.Impulse);
        }

        private void HandleReload()
        {
            if (activeGunLogic == null) return;

            bool canReload = activeGunLogic.CurrentClip < currentWeapon.magazineSize && activeGunLogic.CurrentStash > 0;

            if (Input.GetKeyDown(KeyCode.R) && !isReloading && canReload)
            {
                if (currentWeapon.gunStyle == GunStyle.Primary || currentWeapon.gunStyle == GunStyle.Secondary)
                {
                    StartCoroutine(ReloadCoroutine());
                }
            }
        }

        private IEnumerator ReloadCoroutine()
        {
            isReloading = true;
            if (currentWeapon.gunAudioClips.Length > 2)
            {
                gunReloadAudio.clip = currentWeapon.gunAudioClips[2];
                gunReloadAudio.Play();
                audioEventManager?.NotifyAudioEvent(gunReloadAudio);
            }

            yield return new WaitForSeconds(currentWeapon.reloadTime);

            if (activeGunLogic != null)
            {
                activeGunLogic.Reload();
            }

            isReloading = false;
        }

        private void HandleAimingAndLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * CalculateSensitivity() * Time.timeScale;
            float mouseY = Input.GetAxis("Mouse Y") * CalculateSensitivity() * Time.timeScale;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            transform.localRotation = Quaternion.Euler(0f, mouseX, 0f) * transform.localRotation;
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            if (currentWeaponModel == null) return;

            var positions = currentWeapon.GunTypePositions;
            if (!positions.ContainsKey(currentWeapon.gunType)) return;
            var posData = positions[currentWeapon.gunType];

            bool canAim = Input.GetMouseButton(1) && currentWeapon.gunStyle != GunStyle.Melee && !playerMotor.isRunning && !_isClimbing && !isReloading;

            if (canAim)
            {
                isAiming = true;
                currentWeaponModel.transform.localPosition = Vector3.Lerp(currentWeaponModel.transform.localPosition, posData.aimingPosition, Time.deltaTime * 10f);
                currentWeaponModel.transform.localRotation = Quaternion.Slerp(currentWeaponModel.transform.localRotation, Quaternion.Euler(posData.aimingRotation), Time.deltaTime * 10f);

                if (currentWeapon.gunType == GunType.Sniper)
                {
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40f, Time.deltaTime * 5f);
                    if (sniperCam)
                    {
                        sniperCam.gameObject.SetActive(true);
                        float scroll = Input.GetAxis("Mouse ScrollWheel");
                        dynamicFieldOfView = Mathf.Clamp(dynamicFieldOfView - scroll * 10f, 1f, 25f);
                        sniperCam.fieldOfView = Mathf.Lerp(sniperCam.fieldOfView, dynamicFieldOfView, Time.deltaTime * 5f);
                    }
                }
                else
                {
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50f, Time.deltaTime * 5f);
                }
            }
            else
            {
                isAiming = false;
                currentWeaponModel.transform.localPosition = Vector3.Lerp(currentWeaponModel.transform.localPosition, posData.position, Time.deltaTime * 10f);
                currentWeaponModel.transform.localRotation = Quaternion.Slerp(currentWeaponModel.transform.localRotation, Quaternion.Euler(posData.rotation), Time.deltaTime * 10f);

                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, Time.deltaTime * 5f);
                if (sniperCam) sniperCam.gameObject.SetActive(false);
            }
        }

        private float CalculateSensitivity()
        {
            if (currentWeapon != null && currentWeapon.gunType == GunType.Sniper && isAiming && sniperCam)
            {
                return sensitivity * (cam.fieldOfView / 60f) * (sniperCam.fieldOfView / 40f);
            }
            return sensitivity;
        }

        private bool IsGrenade(GunStyle style)
        {
            return style == GunStyle.Grenade || style == GunStyle.Flashbang || style == GunStyle.Smoke || style == GunStyle.Molotov;
        }
    }
}