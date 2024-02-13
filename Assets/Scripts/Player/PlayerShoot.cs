using RatGamesStudios.OperationDeratization.Equipment;
using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponRecoil recoil;
        [SerializeField] private WeaponSway sway;
        public GameObject muzzleFlash;
        public GameObject impactEffect;
        public GameObject impactRicochet;
        public GameObject bloodSpread;
        [SerializeField] private GameObject bloodWound;
        private AudioSource gunFireAudio;
        private AudioSource gunReloadAudio;
        private AudioSource gunSwitchAudio;
        private PlayerStance currentState = new PlayerStance();
        [HideInInspector] public Camera cam;
        private PlayerMotor playerMotor;
        private PlayerInventory inventory;
        private PlayerStamina stamina;
        private AudioEventManager audioEventManager;

        [Header("Weapon")]
        private Gun currentWeapon;
        private Gun weaponReload;
        private Gun previousWeapon;
        private float autoShotTimer = 0f;
        private float shotTimer = 0f;
        public float throwForce = 25f;
        public float throwUpForce = 10f;
        [SerializeField] [Range(10, 100)] private int linePoints = 25;
        [SerializeField] [Range(0.01f, 0.25f)] private float timeBetweenPoints = 0.03f;
        private LayerMask grenadeCollisionMask;
        private float dynamicFieldOfView;
        private Animator weaponAnimator;
        private Transform weaponHolder;
        private LineRenderer lineRenderer;

        [Header("Movement")]
        private float xRotation = 0f;
        public float sensitivity = 3f;

        [Header("Bool checks")]
        private bool isReloading = false;
        public bool isAiming = false;
        public bool _isClimbing = false;
        private bool isLowQuality = false;

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
            inventory = GetComponent<PlayerInventory>();
            stamina = GetComponent<PlayerStamina>();
            cam = Camera.main;
            lineRenderer = cam.GetComponent<LineRenderer>();
            previousWeapon = inventory.CurrentWeapon;
            currentWeapon = inventory.CurrentWeapon;
            gunFireAudio = transform.Find("Sounds/WeaponFire").GetComponent<AudioSource>();
            gunReloadAudio = transform.Find("Sounds/WeaponReload").GetComponent<AudioSource>();
            gunSwitchAudio = transform.Find("Sounds/WeaponSwitch").GetComponent<AudioSource>();
            sensitivity *= Settings.Sensitivity;
            weaponHolder = transform.Find("Camera/Main Camera/WeaponHolder");

            if (Settings.QualityPreset == 0)
                isLowQuality = true;
        }
        private void Awake()
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            int grenadeLayer = LayerMask.NameToLayer("Ground");
            Physics.IgnoreLayerCollision(grenadeLayer, playerLayer, true);
        }
        private void Update()
        {
            previousWeapon = currentWeapon;
            currentWeapon = inventory.CurrentWeapon;
            _isClimbing = playerMotor._isClimbing;

            if (previousWeapon != null && previousWeapon.gunStyle != currentWeapon.gunStyle)
            {
                gunSwitchAudio.pitch = Random.Range(0.85f, 1.15f);

                if (currentWeapon.gunStyle != GunStyle.Melee && currentWeapon.gunStyle != GunStyle.Grenade && currentWeapon.gunStyle != GunStyle.Flashbang && currentWeapon.gunStyle != GunStyle.Smoke && currentWeapon.gunStyle != GunStyle.Molotov)
                {
                    gunSwitchAudio.PlayOneShot(currentWeapon.gunAudioClips[3]);
                    audioEventManager.NotifyAudioEvent(gunSwitchAudio);
                } 
                else if (currentWeapon.gunStyle != GunStyle.Grenade || currentWeapon.gunStyle != GunStyle.Flashbang || currentWeapon.gunStyle != GunStyle.Smoke || currentWeapon.gunStyle != GunStyle.Molotov)
                {
                    gunSwitchAudio.PlayOneShot(currentWeapon.gunAudioClips[1]);
                    audioEventManager.NotifyAudioEvent(gunSwitchAudio);
                } 
                else
                {
                    gunSwitchAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                    audioEventManager.NotifyAudioEvent(gunSwitchAudio);
                }
            }

            PointerPosition();
            Shoot();

            if (Input.GetKeyDown(KeyCode.R) && currentWeapon.magazineSize != currentWeapon.currentAmmoCount && currentWeapon.maxAmmoCount != 0 && !isReloading &&
                (currentWeapon.gunStyle != GunStyle.Primary || currentWeapon.gunStyle != GunStyle.Secondary))
            {
                weaponReload = currentWeapon;
                StartCoroutine(ReloadCoroutine());
            }

            foreach (Transform child in weaponHolder)
            {
                child.GetChild(0).gameObject.SetActive(true);

                if (child.gameObject.activeSelf)
                {
                    weaponAnimator = child.GetComponent<Animator>();

                    break;
                }
            }
        }
        private void Shoot()
        {
            RaycastHit hit;
            LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Postprocessing"));

            if (Input.GetMouseButtonDown(0) && (Time.time > shotTimer || Time.time > autoShotTimer) && currentWeapon.currentAmmoCount == 0 && !isReloading &&
                (currentWeapon.gunStyle != GunStyle.Grenade || currentWeapon.gunStyle != GunStyle.Flashbang || currentWeapon.gunStyle != GunStyle.Smoke || currentWeapon.gunStyle != GunStyle.Molotov))
            {
                gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
                gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[1]);
                audioEventManager.NotifyAudioEvent(gunFireAudio);

                return;
            }
            if (Input.GetMouseButtonDown(0) && currentWeapon.gunStyle == GunStyle.Melee && !stamina.HasStamina(stamina.attackStaminaCost / 2))
                return;
            if (currentWeapon.gunStyle == GunStyle.Grenade || currentWeapon.gunStyle == GunStyle.Flashbang || currentWeapon.gunStyle == GunStyle.Smoke || currentWeapon.gunStyle == GunStyle.Molotov)
            {
                if (currentWeapon.currentAmmoCount == 0)
                {
                    Transform weapon = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)");
                    currentWeapon.currentAmmoCount = currentWeapon.editorAmmoValue;

                    if (weapon != null)
                        Destroy(weapon.gameObject);
                    if (currentWeapon.gunStyle == GunStyle.Grenade)
                    {
                        inventory.weapons[inventory.currentWeaponIndex] = null;
                        inventory.grenadeWeaponImage.gameObject.SetActive(false);
                    }
                    if (currentWeapon.gunStyle == GunStyle.Flashbang)
                    {
                        inventory.weapons[inventory.currentWeaponIndex] = null;
                        inventory.flashbangWeaponImage.gameObject.SetActive(false);
                    }
                    if (currentWeapon.gunStyle == GunStyle.Smoke)
                    {
                        inventory.weapons[inventory.currentWeaponIndex] = null;
                        inventory.smokeWeaponImage.gameObject.SetActive(false);
                    }
                    if (currentWeapon.gunStyle == GunStyle.Molotov)
                    {
                        inventory.weapons[inventory.currentWeaponIndex] = null;
                        inventory.molotovWeaponImage.gameObject.SetActive(false);
                    }

                    inventory.SetCurrentWeapon(0);
                    inventory.UpdateWeaponImages();

                    return;
                }
            }
            if (Time.time > autoShotTimer && currentWeapon.autoFire && currentWeapon.currentAmmoCount > 0 && !isReloading)
            {
                if (Input.GetMouseButton(0))
                {
                    gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
                    gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                    audioEventManager.NotifyAudioEvent(gunFireAudio);
                    recoil.RecoilFire();
                    currentWeapon.currentAmmoCount--;
                    Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                    ObjectPoolManager.SpawnObject(muzzleFlash, muzzle.position, muzzle.rotation, muzzle);

                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, obstacleMask))
                    {
                        Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                        var hitBox = hit.collider.GetComponent<HitBox>();

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
                            if (hitBox.playerHealth == null)
                            {
                                hitBox.OnRaycastHit(currentWeapon, cam.transform.forward, gameObject);
                                ObjectPoolManager.SpawnObject(bloodSpread, hit.point, impactRotation, hit.collider.transform);

                                if (!isLowQuality)
                                    ObjectPoolManager.SpawnObject(bloodWound, hit.point, impactRotation, hit.collider.transform);
                            }
                        }
                        if (hit.rigidbody != null)
                            hit.rigidbody.AddForce(-hit.normal * currentWeapon.impactForce);
                    }

                    autoShotTimer = Time.time + currentWeapon.timeBetweenShots;
                }
            }
            if (Time.time > shotTimer && !currentWeapon.autoFire && currentWeapon.currentAmmoCount > 0 && !isReloading)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (currentWeapon.gunStyle == GunStyle.Grenade || currentWeapon.gunStyle == GunStyle.Flashbang || currentWeapon.gunStyle == GunStyle.Smoke || currentWeapon.gunStyle == GunStyle.Molotov)
                    {
                        gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
                        gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                        audioEventManager.NotifyAudioEvent(gunFireAudio);
                        currentWeapon.currentAmmoCount--;
                        Vector3 grenadeOffset = new Vector3(0, 0, 0.2f);
                        GameObject grenade = Instantiate(currentWeapon.gunPrefab, weaponHolder.transform.position + grenadeOffset, weaponHolder.transform.rotation);
                        grenade.AddComponent<GrenadeIndicator>();
                        Rigidbody rb = grenade.GetComponent<Rigidbody>();
                        Weapon weaponScript = grenade.GetComponent<Weapon>();
                        Destroy(weaponScript);
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        rb.isKinematic = false;
                        rb.constraints = RigidbodyConstraints.None;
                        rb.freezeRotation = false;

                        if (isAiming)
                            rb.AddForce(weaponHolder.transform.forward * throwForce, ForceMode.Impulse);
                        else
                            rb.AddForce(weaponHolder.transform.forward * throwForce / 2 + weaponHolder.transform.up * throwUpForce / 5, ForceMode.Impulse);
                        if (currentWeapon.gunStyle == GunStyle.Grenade)
                        {
                            Grenade grenadeScript = grenade.GetComponent<Grenade>();
                            grenadeScript.shouldExplode = true;
                        }
                        else if (currentWeapon.gunStyle == GunStyle.Flashbang)
                        {
                            Flashbang flashbangScript = grenade.AddComponent<Flashbang>();
                            flashbangScript.shouldFlash = true;
                        }
                        else if (currentWeapon.gunStyle == GunStyle.Smoke)
                        {
                            Smoke smokeScript = grenade.GetComponent<Smoke>();
                            smokeScript.shouldSmoke = true;
                        }
                        else
                        {
                            Molotov molotovScript = grenade.GetComponent<Molotov>();
                            molotovScript.shouldExplode = true;
                        }
                    }
                    else
                    {
                        gunFireAudio.pitch = Random.Range(0.85f, 1.15f);
                        gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                        audioEventManager.NotifyAudioEvent(gunFireAudio);

                        if (currentWeapon.gunStyle == GunStyle.Primary || currentWeapon.gunStyle == GunStyle.Secondary)
                        {
                            recoil.RecoilFire();
                            currentWeapon.currentAmmoCount--;
                            Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                            ObjectPoolManager.SpawnObject(muzzleFlash, muzzle.position, muzzle.rotation, muzzle);

                            if (currentWeapon.gunType == GunType.Shotgun)
                            {
                                int numPellets = 5;
                                float maxSpread = 0.1f;

                                for (int i = 0; i < numPellets; i++)
                                {
                                    Vector3 spreadDirection = cam.transform.forward + new Vector3(Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread));
                                    RaycastHit spreadHit;

                                    if (Physics.Raycast(cam.transform.position, spreadDirection, out spreadHit, currentWeapon.range, obstacleMask))
                                    {
                                        HitBox spreadHitBox = spreadHit.collider.GetComponent<HitBox>();
                                        ObjectPoolManager.SpawnObject(impactRicochet, spreadHit.point, Quaternion.LookRotation(spreadHit.normal), ObjectPoolManager.PoolType.ParticleSystem);

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
                                        if (spreadHitBox != null && spreadHitBox.playerHealth == null)
                                        {
                                            spreadHitBox.OnRaycastHit(currentWeapon, spreadDirection.normalized, gameObject);

                                            if (!isLowQuality)
                                                ObjectPoolManager.SpawnObject(bloodWound, spreadHit.point, Quaternion.LookRotation(spreadHit.normal), spreadHit.collider.transform);
                                        }
                                        if (spreadHit.rigidbody != null)
                                            spreadHit.rigidbody.AddForce(-spreadHit.normal * currentWeapon.impactForce);
                                    }
                                }
                            }
                        }
                        else
                        {
                            stamina.UseStamina(stamina.attackStaminaCost);
                            stamina.BlockStaminaOnAttack();
                            weaponAnimator.SetTrigger("Attack");
                        }
                        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, obstacleMask))
                        {
                            Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                            HitBox hitBox = hit.collider.GetComponent<HitBox>();

                            if (hitBox == null && currentWeapon.gunStyle != GunStyle.Melee)
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
                            else if (hitBox == null && currentWeapon.gunStyle == GunStyle.Melee)
                                if (hit.collider.CompareTag("Glass"))
                                    hit.collider.GetComponent<Glass>().Break(hit.point, currentWeapon.impactForce);
                            if (hitBox != null && hitBox.playerHealth == null)
                            {
                                hitBox.OnRaycastHit(currentWeapon, cam.transform.forward, gameObject);
                                ObjectPoolManager.SpawnObject(bloodSpread, hit.point, impactRotation, hit.collider.transform);

                                if (!isLowQuality)
                                    if (currentWeapon.gunStyle != GunStyle.Melee)
                                        ObjectPoolManager.SpawnObject(bloodWound, hit.point, impactRotation, hit.collider.transform);
                            }
                            if (hit.rigidbody != null)
                                hit.rigidbody.AddForce(-hit.normal * currentWeapon.impactForce);
                        }
                    }

                    shotTimer = Time.time + currentWeapon.timeBetweenShots;
                }
            }
        }
        private IEnumerator ReloadCoroutine()
        {
            isReloading = true;
            gunReloadAudio.clip = weaponReload.gunAudioClips[2];
            gunReloadAudio.Play();
            audioEventManager.NotifyAudioEvent(gunReloadAudio);

            if (currentWeapon.gunType == GunType.Pistol)
                yield return new WaitForSeconds(2f);
            else if (currentWeapon.gunType == GunType.Revolver || currentWeapon.gunType == GunType.Rifle)
                yield return new WaitForSeconds(3f);
            else if (currentWeapon.gunType == GunType.Shotgun || currentWeapon.gunType == GunType.Sniper)
                yield return new WaitForSeconds(4f);
            if (weaponReload.currentAmmoCount == weaponReload.magazineSize)
                yield break;

            int ammoNeeded = weaponReload.magazineSize - weaponReload.currentAmmoCount;
            int ammoAvailable = Mathf.Min(weaponReload.maxAmmoCount, ammoNeeded);

            if (ammoAvailable == 0)
                yield break;

            int startingAmmoCount = weaponReload.currentAmmoCount;
            int startingMaxAmmoCount = weaponReload.maxAmmoCount;

            if (weaponReload != currentWeapon)
            {
                gunReloadAudio.Stop();
                weaponReload.currentAmmoCount = startingAmmoCount;
                weaponReload.maxAmmoCount = startingMaxAmmoCount;
                isReloading = false;

                yield break;
            }

            weaponReload.currentAmmoCount += ammoAvailable;
            weaponReload.maxAmmoCount -= weaponReload.magazineSize;
            isReloading = false;
        }
        private void PointerPosition()
        {
            //float mouseX = Input.GetAxis("Mouse X") * CalculateSensitivity();
            //float mouseY = Input.GetAxis("Mouse Y") * CalculateSensitivity();
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.timeScale;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.timeScale;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            transform.localRotation = Quaternion.Euler(0f, mouseX, 0f) * transform.localRotation;
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            Transform weapon = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)");
            Vector3 originalPosition = new Vector3(0.05f, -0.0578f, 0.1701f);
            Vector3 originalRotation = new Vector3(20.84f, -161.87f, 100f);
            Vector3 aimingPosition = new Vector3(0.05f, -0.0578f, 0.1701f);
            Vector3 aimingRotation = new Vector3(20.84f, -161.87f, 100f);

            switch (currentWeapon.gunType)
            {
                case GunType.Melee:
                    originalPosition = new Vector3(0.05f, -0.0578f, 0.1701f);
                    originalRotation = new Vector3(20.84f, -161.87f, 100f);
                    break;
                case GunType.Pistol:
                    originalPosition = new Vector3(0.18f, -0.12f, 0.46f);
                    originalRotation = new Vector3(3f, 5f, 0);
                    aimingPosition = new Vector3(0, -0.07f, 0.52f);
                    aimingRotation = new Vector3(0, 0, 0);
                    break;
                case GunType.Revolver:
                    originalPosition = new Vector3(0.19f, -0.22f, 0.35f);
                    originalRotation = new Vector3(-90f, 5f, 0);
                    aimingPosition = new Vector3(0, -0.173f, 0.4f);
                    aimingRotation = new Vector3(-87f, 0, 0);
                    break;
                case GunType.Shotgun:
                    originalPosition = new Vector3(0.16f, -0.23f, 0.44f);
                    originalRotation = new Vector3(3f, 5f, 0);
                    aimingPosition = new Vector3(0.015f, -0.15f, 0.56f);
                    aimingRotation = new Vector3(5f, 0.5f, 0);
                    break;
                case GunType.Rifle:
                    originalPosition = new Vector3(0.16f, -0.27f, 0.25f);
                    originalRotation = new Vector3(3f, 5f, 0);
                    aimingPosition = new Vector3(0, -0.17f, 0.47f);
                    aimingRotation = new Vector3(0, 0, 0);
                    break;
                case GunType.Sniper:
                    originalPosition = new Vector3(0.12f, -0.23f, 0.45f);
                    originalRotation = new Vector3(3f, 5f, 0);
                    aimingPosition = new Vector3(0.0119f, -0.14f, 0.5f);
                    aimingRotation = new Vector3(0, 0, 0);
                    break;
                case GunType.Grenade:
                    originalPosition = new Vector3(0.16f, -0.15f, 0.3f);
                    originalRotation = new Vector3(3f, 0, 0);
                    aimingPosition = new Vector3(0.16f, -0.15f, 0.3f);
                    aimingRotation = new Vector3(3f, 0, 0);
                    break;
                case GunType.Flashbang:
                    originalPosition = new Vector3(0.16f, -0.15f, 0.3f);
                    originalRotation = new Vector3(3f, 0, 0);
                    aimingPosition = new Vector3(0.16f, -0.15f, 0.3f);
                    aimingRotation = new Vector3(3f, 0, 0);
                    break;
                case GunType.Smoke:
                    originalPosition = new Vector3(0.16f, -0.15f, 0.3f);
                    originalRotation = new Vector3(3f, 0, 0);
                    aimingPosition = new Vector3(0.16f, -0.15f, 0.3f);
                    aimingRotation = new Vector3(3f, 0, 0);
                    break;
                case GunType.Molotov:
                    originalPosition = new Vector3(0.12f, -0.16f, 0.17f);
                    originalRotation = new Vector3(3f, 0, 0);
                    aimingPosition = new Vector3(0.12f, -0.16f, 0.17f);
                    aimingRotation = new Vector3(3f, 0, 0);
                    break;
            }

            if (Input.GetMouseButton(1) && currentWeapon.gunStyle != GunStyle.Melee && !playerMotor.isRunning)
            {
                if (!_isClimbing)
                {
                    isAiming = true;
                    weapon.localPosition = aimingPosition;
                    weapon.localRotation = Quaternion.Euler(aimingRotation);

                    if (currentWeapon.gunType == GunType.Sniper)
                    {
                        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40f, Time.deltaTime * 5f);
                        Transform zoom = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/Mesh/SVD/Camera");
                        zoom.gameObject.SetActive(true);
                        Camera zoomCamera = zoom.GetComponent<Camera>();
                        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
                        dynamicFieldOfView = Mathf.Clamp(dynamicFieldOfView - scrollDelta * 10f, 5f, 25f);
                        zoomCamera.fieldOfView = Mathf.Lerp(zoomCamera.fieldOfView, dynamicFieldOfView, Time.deltaTime * 5f);
                    }
                    else if (currentWeapon.gunType == GunType.Grenade || currentWeapon.gunType == GunType.Flashbang || currentWeapon.gunType == GunType.Smoke || currentWeapon.gunType == GunType.Molotov)
                        DrawTrajectory();
                    else
                        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40f, Time.deltaTime * 5f);
                }
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, Time.deltaTime * 5f);
                isAiming = false;
                lineRenderer.enabled = false;

                if (weapon != null)
                {
                    weapon.localPosition = originalPosition;
                    weapon.localRotation = Quaternion.Euler(originalRotation);
                }
                if (currentWeapon.gunType == GunType.Sniper)
                {
                    Transform zoom = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/Mesh/SVD/Camera");
                    zoom.gameObject.SetActive(false);
                }
                if (currentState.playerStance == PlayerStance.Stance.Idle || currentState.playerStance == PlayerStance.Stance.Walking)
                    playerMotor.moveSpeed = 4f;
                else
                    playerMotor.moveSpeed = 2f;
            }
        }
        /*private float CalculateSensitivity()
        {
            float baseSensitivity = xSensitivity;

            if (currentWeapon.gunType == GunType.Sniper)
            {
                Transform zoom = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/Mesh/SVD/Camera");
                Camera zoomCamera = zoom.GetComponent<Camera>();
                float adjustedSensitivity = baseSensitivity * (cam.fieldOfView / 60f) * (zoomCamera.fieldOfView / 40f);

                return adjustedSensitivity;
            }

            return baseSensitivity;
        }*/
        private void DrawTrajectory()
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints + 1);
            Vector3 grenadeOffset = new Vector3(0, 0, 0.2f);
            Vector3 startPosition = weaponHolder.transform.position + grenadeOffset;
            Vector3 startVelocity = throwForce * weaponHolder.transform.forward / 2f;
            int i = 0;
            lineRenderer.SetPosition(i, startPosition);

            for (float time = 0; time < linePoints; time += timeBetweenPoints)
            {
                i++;
                Vector3 point = startPosition + time * startVelocity;
                point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
                lineRenderer.SetPosition(i, point);
                Vector3 lastPosition = lineRenderer.GetPosition(i - 1);

                if (Physics.Raycast(lastPosition, (point - lastPosition).normalized, out RaycastHit hit, (point - lastPosition).magnitude, grenadeCollisionMask))
                {
                    lineRenderer.SetPosition(i, hit.point);
                    lineRenderer.positionCount = i + 1;

                    return;
                }
            }
        }
    }
}