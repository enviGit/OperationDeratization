using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Camera cam;
    private PlayerStance currentState = new PlayerStance();

    [Header("Movement")]
    private Vector3 playerVelocity;
    public float gravity = -9.8f;
    public float jumpHeight = 0.7f;
    public float moveSpeed = 4f;
    private float xRotation = 0f;
    public float xSensitivity = 3f;
    public float ySensitivity = 3f;

    [Header("Weapon")]
    private Gun currentWeapon;
    private Gun weaponReload;
    private Gun previousWeapon;
    [SerializeField] private WeaponRecoil recoil;
    private float autoShotTimer = 0f;
    private float shotTimer = 0f;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject impactRicochet;
    public AudioSource gunAudio;
    public float throwForce = 25f;
    public float throwUpForce = 10f;
    [SerializeField] [Range(10, 100)] private int linePoints = 25;
    [SerializeField] [Range(0.01f, 0.25f)] private float timeBetweenPoints = 0.1f;
    private LayerMask grenadeCollisionMask;

    [Header("Fall damage")]
    public float fallDamageMultiplier = 1.5f;
    private float fallTime = 0f;
    private float fallDamageTaken;
    private float fallTimeCalc = 0.7f;

    [Header("Bool checks")]
    public bool isGrounded;
    public bool isCrouching = false;
    private bool isReloading = false;
    public bool isAiming = false;
    public bool isMoving = false;
    public bool isRunning = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        currentState = new PlayerStance();
        currentState.playerStance = PlayerStance.Stance.Idle;
        currentState.camHeight = 2f;
        previousWeapon = GetComponent<PlayerInventory>().CurrentWeapon;
        currentWeapon = GetComponent<PlayerInventory>().CurrentWeapon;
    }
    private void Awake()
    {
        int grenadeLayer = FindObjectOfType<Grenade>().gameObject.layer;

        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(grenadeLayer, i))
                grenadeCollisionMask |= 1 << i;
        }
    }
    private void Update()
    {
        previousWeapon = currentWeapon;
        currentWeapon = GetComponent<PlayerInventory>().CurrentWeapon;

        if (previousWeapon != null && previousWeapon.gunStyle != currentWeapon.gunStyle)
        {
            if (currentWeapon.gunStyle != GunStyle.Melee && currentWeapon.gunStyle != GunStyle.Grenade && currentWeapon.gunStyle != GunStyle.Flashbang && currentWeapon.gunStyle != GunStyle.Smoke)
            {
                gunAudio.clip = currentWeapon.gunAudioClips[3];
                gunAudio.Play();
            }
            else if (currentWeapon.gunStyle != GunStyle.Grenade || currentWeapon.gunStyle != GunStyle.Flashbang || currentWeapon.gunStyle != GunStyle.Smoke)
            {
                //

                //
            }
            else
            {
                gunAudio.clip = currentWeapon.gunAudioClips[0];
                gunAudio.Play();
            }
        }

        isGrounded = controller.isGrounded;

        Move();
        Crouch();
        CrouchToggle();
        Jump();
        Gravity();
        Shoot();
        PointerPosition();

        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.gunStyle != GunStyle.Melee && currentWeapon.magazineSize != currentWeapon.currentAmmoCount && currentWeapon.maxAmmoCount != 0)
        {
            weaponReload = currentWeapon;
            StartCoroutine(ReloadCoroutine());
        }

        switch (currentWeapon.gunType)
        {
            case GunType.Pistol:
                moveSpeed *= 0.9f;
                break;
            case GunType.Revolver:
                moveSpeed *= 0.9f;
                break;
            case GunType.Rifle:
                moveSpeed *= 0.75f;
                break;
            case GunType.Sniper:
                moveSpeed *= 0.6f;
                break;
            default:
                moveSpeed *= 1f;
                break;
        }
    }
    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        if (moveDirection.magnitude > 0)
        {
            isMoving = true;

            if (isCrouching)
                currentState.playerStance = PlayerStance.Stance.Crouching;
            else
                currentState.playerStance = PlayerStance.Stance.Walking;
        }
        else
        {
            isMoving = false;

            if (isCrouching)
                currentState.playerStance = PlayerStance.Stance.Crouching;
            else
                currentState.playerStance = PlayerStance.Stance.Idle;
        }

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isCrouching && GetComponent<PlayerStamina>().currentStamina > 0f)
        {
            isRunning = true;
            moveSpeed *= 1.3f;
        }
        else
        {
            isRunning = false;
            moveSpeed /= 1.3f;
            moveSpeed = Mathf.Clamp(moveSpeed, 1f, 4f);
        }

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    private void Gravity()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (FindObjectOfType<LadderTrigger>().isClimbing)
                isGrounded = false;

            fallTime = 0f;
            playerVelocity.y = -2f;

            if (fallDamageTaken > 0)
            {
                //Debug.Log(fallDamageTaken);
                GetComponent<PlayerHealth>().TakeFallingDamage(fallDamageTaken);
                fallDamageTaken = 0;
            }
        }
        else
        {
            if (!FindObjectOfType<LadderTrigger>().isClimbing)
                fallTime += Time.deltaTime;

            if (fallTime > fallTimeCalc)
            {
                float fallDamage = fallTime * fallDamageMultiplier;

                if (fallDamage > 0)
                    fallDamageTaken += fallDamage;
            }
        }
    }
    private void Crouch()
    {
        if (currentState.playerStance == PlayerStance.Stance.Crouching)
        {
            float camNewHeight = Mathf.Lerp(Camera.main.transform.localPosition.y, 1f, Time.deltaTime * 5f);
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, camNewHeight, Camera.main.transform.localPosition.z);
            controller.height = 1f;
            moveSpeed = 2f;
        }
        else
        {
            float camNewHeight = Mathf.Lerp(Camera.main.transform.localPosition.y, 2f, Time.deltaTime * 5f);
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, camNewHeight, Camera.main.transform.localPosition.z);
            controller.height = 2f;
            moveSpeed = 4f;
        }
    }
    private void CrouchToggle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isGrounded)
                return;
            if (currentState.playerStance == PlayerStance.Stance.Crouching)
            {
                RaycastHit hit;
                LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

                if (Physics.Raycast(transform.position, transform.up, out hit, controller.height, obstacleMask))
                {
                    isCrouching = true;
                    currentState.playerStance = PlayerStance.Stance.Crouching;
                    currentState.camHeight = 1f;
                }
                else
                {
                    isCrouching = false;
                    currentState.playerStance = PlayerStance.Stance.Idle;
                    currentState.camHeight = 2f;
                }
            }
            else
            {
                isCrouching = true;
                currentState.playerStance = PlayerStance.Stance.Crouching;
                currentState.camHeight = 1f;
            }
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && GetComponent<PlayerStamina>().currentStamina >= GetComponent<PlayerStamina>().jumpStaminaCost / 2)
        {
            RaycastHit hit;
            LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

            if (Physics.Raycast(transform.position, transform.up, out hit, controller.height, obstacleMask))
                return;
            if (FindObjectOfType<LadderTrigger>().isClimbing)
                FindObjectOfType<LadderTrigger>().DetachFromLadder();
            if (isCrouching)
            {
                isCrouching = false;
                currentState.playerStance = PlayerStance.Stance.Idle;
                currentState.camHeight = 2f;
            }

            fallTimeCalc = 1.2f;
            fallDamageMultiplier = 0.6f;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            if (isGrounded)
            {
                fallTimeCalc = 0.7f;
                fallDamageMultiplier = 1.5f;
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
            gunAudio.clip = currentWeapon.gunAudioClips[1];
            gunAudio.Play();
            return;
        }
        if (Input.GetMouseButtonDown(0) && currentWeapon.gunStyle == GunStyle.Melee && !(GetComponent<PlayerStamina>().currentStamina >= GetComponent<PlayerStamina>().attackStaminaCost / 2))
            return;
        if (currentWeapon.gunStyle == GunStyle.Grenade || currentWeapon.gunStyle == GunStyle.Flashbang || currentWeapon.gunStyle == GunStyle.Smoke)
        {
            if (currentWeapon.currentAmmoCount == 0)
            {
                Transform weapon = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)");
                currentWeapon.currentAmmoCount = currentWeapon.magazineSize;
                GetComponent<PlayerInventory>().isPickable = true;

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
                gunAudio.clip = currentWeapon.gunAudioClips[0];
                gunAudio.Play();
                recoil.RecoilFire();
                currentWeapon.currentAmmoCount--;
                Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                ParticleSystem flash = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
                flash.transform.SetParent(muzzle);
                flash.Play();

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currentWeapon.range, obstacleMask))
                {
                    //Debug.Log("Hit: " + hit.collider.name);
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    Quaternion impactRotation = Quaternion.LookRotation(hit.normal);

                    if (damageable == null)
                    {
                        GameObject ricochet = Instantiate(impactRicochet, hit.point, impactRotation);
                        Destroy(ricochet, 2f);

                        if (!hit.collider.gameObject.GetComponent<Weapon>())
                        {
                            GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);

                            if (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable") || hit.collider.CompareTag("MovingDoors"))
                                impact.transform.SetParent(hit.collider.transform);
                        }
                    }
                    if (hit.rigidbody != null)
                        hit.rigidbody.AddForce(-hit.normal * currentWeapon.impactForce);
                    if (damageable != null)
                        damageable.DealDamage(Random.Range(currentWeapon.minimumDamage, currentWeapon.maximumDamage));
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
                    //gunAudio.clip = currentWeapon.gunAudioClips[0];
                    //gunAudio.Play();
                    currentWeapon.currentAmmoCount--;
                    Transform weaponHolder = transform.Find("Camera/Main Camera/WeaponHolder");
                    Vector3 grenadeOffset = new Vector3(0, 0, 0.2f);
                    GameObject grenade = Instantiate(currentWeapon.gunPrefab, weaponHolder.transform.position + grenadeOffset, weaponHolder.transform.rotation);
                    grenade.AddComponent<GrenadeIndicator>();
                    Rigidbody rb = grenade.GetComponent<Rigidbody>();
                    Weapon weaponScript = grenade.GetComponent<Weapon>();
                    Destroy(weaponScript);
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = false;
                    rb.freezeRotation = false;
                    rb.transform.SetParent(null, true);

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
                    gunAudio.clip = currentWeapon.gunAudioClips[0];
                    gunAudio.Play();

                    if (currentWeapon.gunStyle == GunStyle.Primary || currentWeapon.gunStyle == GunStyle.Secondary)
                    {
                        recoil.RecoilFire();
                        currentWeapon.currentAmmoCount--;
                        Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                        ParticleSystem flash = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
                        flash.transform.SetParent(muzzle);
                        flash.Play();
                    }
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currentWeapon.range, obstacleMask))
                    {
                        //Debug.Log("Hit: " + hit.collider.name);
                        IDamageable damageable = hit.collider.GetComponent<IDamageable>();

                        Quaternion impactRotation = Quaternion.LookRotation(hit.normal);

                        if (damageable == null)
                        {
                            GameObject ricochet = Instantiate(impactRicochet, hit.point, impactRotation);
                            Destroy(ricochet, 2f);

                            if (!hit.collider.gameObject.GetComponent<Weapon>())
                            {
                                GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);

                                if (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable") || hit.collider.CompareTag("MovingDoors"))
                                    impact.transform.SetParent(hit.collider.transform);
                            }
                        }
                        if (hit.rigidbody != null)
                            hit.rigidbody.AddForce(-hit.normal * currentWeapon.impactForce);
                        if (damageable != null)
                            damageable.DealDamage(Random.Range(currentWeapon.minimumDamage, currentWeapon.maximumDamage));
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
            gunAudio.clip = weaponReload.gunAudioClips[2];
            gunAudio.Play();
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
            gunAudio.Stop();
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
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(0f, mouseX, 0f) * transform.localRotation;
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Transform weapon = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)");
        Vector3 originalPosition = new Vector3(0.05f, -0.08f, 0.2f);
        Vector3 originalRotation = new Vector3(5.2f, -125, 101);
        Vector3 aimingPosition = new Vector3(0.05f, -0.08f, 0.2f);
        Vector3 aimingRotation = new Vector3(5.2f, -125, 101);

        switch (currentWeapon.gunType)
        {
            case GunType.Melee:
                originalPosition = new Vector3(0.05f, -0.08f, 0.2f);
                originalRotation = new Vector3(5.2f, -125, 101);
                break;
            case GunType.Pistol:
                originalPosition = new Vector3(0.16f, -0.15f, 0.3f);
                originalRotation = new Vector3(3f, 0, 0);
                aimingPosition = new Vector3(0, -0.07f, 0.24f);
                aimingRotation = new Vector3(0, 0, 0);
                break;
            case GunType.Revolver:
                originalPosition = new Vector3(0.12f, -0.24f, 0.2f);
                originalRotation = new Vector3(-90f, 0, 0);
                aimingPosition = new Vector3(0, -0.173f, 0.2f);
                aimingRotation = new Vector3(-87f, 0, 0);
                break;
            case GunType.Shotgun:
                originalPosition = new Vector3(0.16f, -0.25f, 0.5f);
                originalRotation = new Vector3(3f, 0, 0);
                aimingPosition = new Vector3(0.015f, -0.15f, 0.5f);
                aimingRotation = new Vector3(5f, 0.5f, 0);
                break;
            case GunType.Rifle:
                originalPosition = new Vector3(0.16f, -0.25f, 0.5f);
                originalRotation = new Vector3(3f, 0, 0);
                aimingPosition = new Vector3(0, -0.17f, 0.24f);
                aimingRotation = new Vector3(0, 0, 0);
                break;
            case GunType.Sniper:
                originalPosition = new Vector3(0.16f, -0.2f, 0.6f);
                originalRotation = new Vector3(3f, 0, 0);
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

        if (Input.GetMouseButton(1) && currentWeapon.gunStyle != GunStyle.Melee && !isRunning && !FindObjectOfType<LadderTrigger>().isClimbing)
        {
            isAiming = true;
            weapon.localPosition = aimingPosition;
            weapon.localRotation = Quaternion.Euler(aimingRotation);

            if (currentState.playerStance == PlayerStance.Stance.Idle || currentState.playerStance == PlayerStance.Stance.Walking)
                moveSpeed = 2f;
            else
                moveSpeed = 1f;
            if (currentWeapon.gunType == GunType.Sniper)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40f, Time.deltaTime * 5f);

                weapon.localPosition = aimingPosition;
                weapon.localRotation = Quaternion.Euler(aimingRotation);

                if (currentState.playerStance == PlayerStance.Stance.Idle || currentState.playerStance == PlayerStance.Stance.Walking)
                    moveSpeed = 2f;
                else
                    moveSpeed = 1f;

                xSensitivity = 1f;
                ySensitivity = 1f;
                Transform zoom = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/Mesh/SVD/Camera");
                Camera zoomCamera = zoom.GetComponent<Camera>();
                float newFieldOfView = zoomCamera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * 25f;
                newFieldOfView = Mathf.Clamp(newFieldOfView, 1f, 6f);
                zoomCamera.fieldOfView = newFieldOfView;
            }
            else if (currentWeapon.gunType == GunType.Grenade || currentWeapon.gunType == GunType.Flashbang || currentWeapon.gunType == GunType.Smoke)
                DrawTrajectory();
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40f, Time.deltaTime * 5f);
                xSensitivity = 3f;
                ySensitivity = 3f;
            }
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, Time.deltaTime * 5f);
            isAiming = false;
            LineRenderer lineRenderer = cam.GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
            weapon.localPosition = originalPosition;
            weapon.localRotation = Quaternion.Euler(originalRotation);

            if (currentState.playerStance == PlayerStance.Stance.Idle || currentState.playerStance == PlayerStance.Stance.Walking)
                moveSpeed = 4f;
            else
                moveSpeed = 2f;

            xSensitivity = 3f;
            ySensitivity = 3f;
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