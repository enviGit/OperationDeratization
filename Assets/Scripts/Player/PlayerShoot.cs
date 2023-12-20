using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponRecoil recoil;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject impactRicochet;
    public GameObject bloodSpread;
    private AudioSource gunFireAudio;
    private AudioSource gunReloadAudio;
    private AudioSource gunSwitchAudio;
    private PlayerStance currentState = new PlayerStance();
    public Camera cam;
    private PlayerMotor playerMotor;
    private LadderTrigger ladder;
    private PlayerStamina stamina;
    private GameObject parentObject;

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
    private Transform weaponHolderActive;

    [Header("Movement")]
    private float xRotation = 0f;
    public float xSensitivity = 3f;
    public float ySensitivity = 3f;

    [Header("Bool checks")]
    private bool isReloading = false;
    public bool isAiming = false;
    public bool _isClimbing = false;

    private void Start()
    {
        ladder = FindObjectOfType<LadderTrigger>();
        playerMotor = GetComponent<PlayerMotor>();
        stamina = GetComponent<PlayerStamina>();
        cam = Camera.main;
        previousWeapon = GetComponent<PlayerInventory>().CurrentWeapon;
        currentWeapon = GetComponent<PlayerInventory>().CurrentWeapon;
        parentObject = GameObject.Find("3D");
        gunFireAudio = transform.Find("Sounds/WeaponFire").GetComponent<AudioSource>();
        gunReloadAudio = transform.Find("Sounds/WeaponReload").GetComponent<AudioSource>();
        gunSwitchAudio = transform.Find("Sounds/WeaponSwitch").GetComponent<AudioSource>();
        xSensitivity *= Settings.Sensitivity;
        ySensitivity *= Settings.Sensitivity;
        weaponHolderActive = transform.Find("Camera/Main Camera/WeaponHolder");
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
        currentWeapon = GetComponent<PlayerInventory>().CurrentWeapon;

        if (ladder != null)
            _isClimbing = ladder.isClimbing;
        if (previousWeapon != null && previousWeapon.gunStyle != currentWeapon.gunStyle)
        {
            if (currentWeapon.gunStyle != GunStyle.Melee && currentWeapon.gunStyle != GunStyle.Grenade && currentWeapon.gunStyle != GunStyle.Flashbang && currentWeapon.gunStyle != GunStyle.Smoke)
                gunSwitchAudio.PlayOneShot(currentWeapon.gunAudioClips[3]);
            else if (currentWeapon.gunStyle != GunStyle.Grenade || currentWeapon.gunStyle != GunStyle.Flashbang || currentWeapon.gunStyle != GunStyle.Smoke)
                gunSwitchAudio.PlayOneShot(currentWeapon.gunAudioClips[1]);
            else
                gunSwitchAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
        }

        PointerPosition();
        Shoot();

        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.gunStyle != GunStyle.Melee && currentWeapon.magazineSize != currentWeapon.currentAmmoCount && currentWeapon.maxAmmoCount != 0 && !isReloading)
        {
            weaponReload = currentWeapon;
            StartCoroutine(ReloadCoroutine());
        }

        foreach (Transform child in weaponHolderActive)
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
        LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

        if (Input.GetMouseButtonDown(0) && (Time.time > shotTimer || Time.time > autoShotTimer) && currentWeapon.currentAmmoCount == 0 && !isReloading &&
            (currentWeapon.gunStyle != GunStyle.Grenade || currentWeapon.gunStyle != GunStyle.Flashbang || currentWeapon.gunStyle != GunStyle.Smoke))
        {
            gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[1]);
            return;
        }
        if (Input.GetMouseButtonDown(0) && currentWeapon.gunStyle == GunStyle.Melee && !stamina.HasStamina(stamina.attackStaminaCost / 2))
            return;
        if (currentWeapon.gunStyle == GunStyle.Grenade || currentWeapon.gunStyle == GunStyle.Flashbang || currentWeapon.gunStyle == GunStyle.Smoke)
        {
            if (currentWeapon.currentAmmoCount == 0)
            {
                Transform weapon = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)");
                currentWeapon.currentAmmoCount = currentWeapon.editorAmmoValue;

                if (weapon != null)
                    Destroy(weapon.gameObject);
                if (currentWeapon.gunStyle == GunStyle.Grenade)
                {
                    GetComponent<PlayerInventory>().weapons[GetComponent<PlayerInventory>().currentWeaponIndex] = null;
                    GetComponent<PlayerInventory>().grenadeWeaponImage.gameObject.SetActive(false);
                }
                if (currentWeapon.gunStyle == GunStyle.Flashbang)
                {
                    GetComponent<PlayerInventory>().weapons[GetComponent<PlayerInventory>().currentWeaponIndex] = null;
                    GetComponent<PlayerInventory>().flashbangWeaponImage.gameObject.SetActive(false);
                }
                if (currentWeapon.gunStyle == GunStyle.Smoke)
                {
                    GetComponent<PlayerInventory>().weapons[GetComponent<PlayerInventory>().currentWeaponIndex] = null;
                    GetComponent<PlayerInventory>().smokeWeaponImage.gameObject.SetActive(false);
                }

                GetComponent<PlayerInventory>().SetCurrentWeapon(0);
                GetComponent<PlayerInventory>().UpdateWeaponImages();

                return;
            }
        }
        if (Time.time > autoShotTimer && currentWeapon.autoFire && currentWeapon.currentAmmoCount > 0 && !isReloading)
        {
            if (Input.GetMouseButton(0))
            {
                gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                recoil.RecoilFire();
                currentWeapon.currentAmmoCount--;
                Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                ParticleSystem flash = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation, muzzle);
                flash.Play();
                Destroy(flash, 1f);

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currentWeapon.range, obstacleMask))
                {
                    // Debug.Log("Hit: " + hit.collider.name);
                    Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                    var hitBox = hit.collider.GetComponent<HitBox>();

                    if (hitBox == null)
                    {
                        GameObject ricochet = Instantiate(impactRicochet, hit.point, impactRotation);
                        Destroy(ricochet, 2f);

                        if (hit.collider.gameObject.GetComponent<Weapon>() == null && !hit.collider.CompareTag("GraveyardWall") && !hit.collider.CompareTag("MovingDoors") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Postprocessing"))
                        {
                            GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);

                            if (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                                impact.transform.SetParent(hit.collider.transform);
                        }
                    }
                    else
                    {
                        hitBox.OnRaycastHit(currentWeapon, Camera.main.transform.forward, gameObject);
                        Instantiate(bloodSpread, hit.point, impactRotation, hit.collider.transform);
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
                if (currentWeapon.gunStyle == GunStyle.Grenade || currentWeapon.gunStyle == GunStyle.Flashbang || currentWeapon.gunStyle == GunStyle.Smoke)
                {
                    gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);
                    currentWeapon.currentAmmoCount--;
                    Transform weaponHolder = transform.Find("Camera/Main Camera/WeaponHolder");
                    Vector3 grenadeOffset = new Vector3(0, 0, 0.2f);
                    GameObject grenade = Instantiate(currentWeapon.gunPrefab, weaponHolder.transform.position + grenadeOffset, weaponHolder.transform.rotation, parentObject.transform);
                    grenade.AddComponent<GrenadeIndicator>();
                    Rigidbody rb = grenade.GetComponent<Rigidbody>();
                    Weapon weaponScript = grenade.GetComponent<Weapon>();
                    Destroy(weaponScript);
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = false;
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
                    else
                    {
                        Smoke smokeScript = grenade.GetComponent<Smoke>();
                        smokeScript.shouldSmoke = true;
                    }
                }
                else
                {
                    gunFireAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);

                    if (currentWeapon.gunStyle == GunStyle.Primary || currentWeapon.gunStyle == GunStyle.Secondary)
                    {
                        recoil.RecoilFire();
                        currentWeapon.currentAmmoCount--;
                        Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                        ParticleSystem flash = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation, muzzle);
                        flash.Play();
                        Destroy(flash, 1f);
                    }
                    else
                    {
                        stamina.UseStamina(stamina.attackStaminaCost);
                        stamina.BlockStaminaOnAttack();
                        weaponAnimator.SetTrigger("Attack");
                    }
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currentWeapon.range, obstacleMask))
                    {
                        //Debug.Log("Hit: " + hit.collider.name);
                        Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
                        var hitBox = hit.collider.GetComponent<HitBox>();

                        if (hitBox == null && currentWeapon.gunStyle != GunStyle.Melee)
                        {
                            GameObject ricochet = Instantiate(impactRicochet, hit.point, impactRotation);
                            Destroy(ricochet, 2f);

                            if (hit.collider.gameObject.GetComponent<Weapon>() == null && !hit.collider.CompareTag("GraveyardWall") && !hit.collider.CompareTag("MovingDoors") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Postprocessing"))
                            {
                                GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);

                                if (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                                    impact.transform.SetParent(hit.collider.transform);
                            }
                        }
                        if (hitBox != null)
                        {
                            hitBox.OnRaycastHit(currentWeapon, Camera.main.transform.forward, gameObject);
                            Instantiate(bloodSpread, hit.point, impactRotation, hit.collider.transform);
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

        if (currentWeapon.gunStyle != GunStyle.Grenade && currentWeapon.gunStyle != GunStyle.Flashbang && currentWeapon.gunStyle != GunStyle.Smoke)
        {
            gunReloadAudio.clip = weaponReload.gunAudioClips[2];
            gunReloadAudio.Play();
        }
        if (currentWeapon.gunType == GunType.Pistol)
            yield return new WaitForSeconds(2f);
        else if (currentWeapon.gunType == GunType.Revolver)
            yield return new WaitForSeconds(3f);
        else if (currentWeapon.gunType == GunType.Shotgun)
            yield return new WaitForSeconds(4f);
        else if (currentWeapon.gunType == GunType.Rifle)
            yield return new WaitForSeconds(3f);
        else if (currentWeapon.gunType == GunType.Sniper)
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
        float mouseX;
        float mouseY;

        if (isAiming && currentWeapon.gunType == GunType.Sniper)
        {
            float sensitivityModifier = 1f;

            switch (dynamicFieldOfView)
            {
                case 1f:
                    sensitivityModifier = 0.2f;
                    break;
                case 3.5f:
                    sensitivityModifier = 0.5f;
                    break;
                case 6f:
                    sensitivityModifier = 1f;
                    break;
                default:
                    sensitivityModifier = 1f;
                    break;
            }

            mouseX = Input.GetAxis("Mouse X") * (xSensitivity * sensitivityModifier);
            mouseY = Input.GetAxis("Mouse Y") * (ySensitivity * sensitivityModifier);
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * xSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * ySensitivity;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(0f, mouseX, 0f) * transform.localRotation;
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Transform weapon = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)");
        Vector3 originalPosition = new Vector3(0.0866f, -0.02f, 0.1845f);
        Vector3 originalRotation = new Vector3(20.84f, 198.13f, 129.6f);
        Vector3 aimingPosition = new Vector3(0.05f, -0.08f, 0.2f);
        Vector3 aimingRotation = new Vector3(5.2f, -125, 101);

        switch (currentWeapon.gunType)
        {
            case GunType.Melee:
                originalPosition = new Vector3(0.0866f, -0.02f, 0.1845f);
                originalRotation = new Vector3(20.84f, 198.13f, 129.6f);
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
                aimingPosition = new Vector3(0.0119f, -0.13355f, 0.42f);
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
                    Camera zoomCamera = zoom.GetComponent<Camera>();
                    dynamicFieldOfView = zoomCamera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * 25f;
                    dynamicFieldOfView = Mathf.Clamp(dynamicFieldOfView, 1f, 6f);
                    zoomCamera.fieldOfView = dynamicFieldOfView;
                }
                else if (currentWeapon.gunType == GunType.Grenade || currentWeapon.gunType == GunType.Flashbang || currentWeapon.gunType == GunType.Smoke)
                    DrawTrajectory();
                else
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40f, Time.deltaTime * 5f);
            }
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, Time.deltaTime * 5f);
            isAiming = false;
            LineRenderer lineRenderer = cam.GetComponent<LineRenderer>();
            lineRenderer.enabled = false;

            if (weapon != null)
            {
                weapon.localPosition = originalPosition;
                weapon.localRotation = Quaternion.Euler(originalRotation);
            }
            if (currentState.playerStance == PlayerStance.Stance.Idle || currentState.playerStance == PlayerStance.Stance.Walking)
                playerMotor.moveSpeed = 4f;
            else
                playerMotor.moveSpeed = 2f;
        }
    }
    private void DrawTrajectory()
    {
        Transform weaponHolder = transform.Find("Camera/Main Camera/WeaponHolder");
        LineRenderer lineRenderer = cam.GetComponent<LineRenderer>();
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
