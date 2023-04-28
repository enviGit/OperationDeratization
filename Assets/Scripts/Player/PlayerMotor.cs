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
    private void Update()
    {
        previousWeapon = currentWeapon;
        currentWeapon = GetComponent<PlayerInventory>().CurrentWeapon;

        if (previousWeapon != null && previousWeapon.gunStyle != currentWeapon.gunStyle)
        {
            if (currentWeapon.gunStyle != GunStyle.Melee)
            {
                gunAudio.clip = currentWeapon.gunAudioClips[3];
                gunAudio.Play();
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
        Climb();
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
    private void Climb()
    {
        if (Input.GetKey(KeyCode.E) && !isGrounded)
        {
            float climbSpeed = 3f;
            Vector3 climbDirection = transform.up * climbSpeed * Time.deltaTime;
            controller.Move(climbDirection);
        }
    }
    private void Shoot()
    {
        RaycastHit hit;
        LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

        if (Input.GetMouseButtonDown(0) && (Time.time > shotTimer || Time.time > autoShotTimer) && currentWeapon.currentAmmoCount == 0 && !isReloading)
        {
            gunAudio.clip = currentWeapon.gunAudioClips[1];
            gunAudio.Play();
            return;
        }
        if (Input.GetMouseButtonDown(0) && currentWeapon.gunStyle == GunStyle.Melee && !(GetComponent<PlayerStamina>().currentStamina >= GetComponent<PlayerStamina>().attackStaminaCost / 2))
            return;
        if (Time.time > autoShotTimer && currentWeapon.autoFire && currentWeapon.currentAmmoCount > 0 && !isReloading)
        {
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currentWeapon.range, obstacleMask))
                {
                    //Debug.Log("Hit: " + hit.collider.name);
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    gunAudio.clip = currentWeapon.gunAudioClips[0];
                    gunAudio.Play();
                    recoil.RecoilFire();
                    currentWeapon.currentAmmoCount--;
                    Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                    ParticleSystem flash = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
                    flash.transform.SetParent(muzzle);
                    flash.Play();
                    Quaternion impactRotation = Quaternion.LookRotation(hit.normal);

                    if (damageable == null)
                    {
                        GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);
                        GameObject ricochet = Instantiate(impactRicochet, hit.point, impactRotation);

                        if (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                            impact.transform.SetParent(hit.collider.transform);

                        Destroy(ricochet, 2f);
                        Destroy(impact, 5f);
                    }
                    if (hit.rigidbody != null)
                        hit.rigidbody.AddForce(-hit.normal * currentWeapon.impactForce);
                    if (damageable != null)
                        damageable.DealDamage(Random.Range(currentWeapon.minimumDamage, currentWeapon.maximumDamage));
                }
                else
                {
                    gunAudio.clip = currentWeapon.gunAudioClips[0];
                    gunAudio.Play();
                    recoil.RecoilFire();
                    currentWeapon.currentAmmoCount--;
                    Transform muzzle = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                    ParticleSystem flash = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation);
                    flash.transform.SetParent(muzzle);
                    flash.Play();
                }

                autoShotTimer = Time.time + currentWeapon.timeBetweenShots;
            }

        }
        if (Time.time > shotTimer && !currentWeapon.autoFire && currentWeapon.currentAmmoCount > 0 && !isReloading)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currentWeapon.range, obstacleMask))
                {
                    //Debug.Log("Hit: " + hit.collider.name);
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
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
                        Quaternion impactRotation = Quaternion.LookRotation(hit.normal);

                        if (damageable == null)
                        {
                            GameObject ricochet = Instantiate(impactRicochet, hit.point, impactRotation);
                            GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);

                            if (hit.rigidbody != null || hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                                impact.transform.SetParent(hit.collider.transform);

                            Destroy(ricochet, 2f);
                            Destroy(impact, 5f);
                        }
                    }
                    if (hit.rigidbody != null)
                        hit.rigidbody.AddForce(-hit.normal * currentWeapon.impactForce);
                    if (damageable != null)
                        damageable.DealDamage(Random.Range(currentWeapon.minimumDamage, currentWeapon.maximumDamage));
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
                }

                shotTimer = Time.time + currentWeapon.timeBetweenShots;
            }
        }
    }
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        gunAudio.clip = weaponReload.gunAudioClips[2];
        gunAudio.Play();

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
        }

        if (Input.GetMouseButton(1) && currentWeapon.gunStyle != GunStyle.Melee && !isRunning)
        {
            if(currentWeapon.gunType == GunType.Sniper)
            {
                xSensitivity = 1f;
                ySensitivity = 1f;
                Transform zoom = transform.Find("Camera/Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/Mesh/SVD/Camera");
                Camera zoomCamera = zoom.GetComponent<Camera>();
                float newFieldOfView = zoomCamera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * 25f;
                newFieldOfView = Mathf.Clamp(newFieldOfView, 1f, 6f);
                zoomCamera.fieldOfView = newFieldOfView;
            }
            else
            {
                xSensitivity = 3f;
                ySensitivity = 3f;
            }

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40f, Time.deltaTime * 5f);
            isAiming = true;
            weapon.localPosition = aimingPosition;
            weapon.localRotation = Quaternion.Euler(aimingRotation);

            if (currentState.playerStance == PlayerStance.Stance.Idle || currentState.playerStance == PlayerStance.Stance.Walking)
                moveSpeed = 2f;
            else
                moveSpeed = 1f;
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, Time.deltaTime * 5f);
            isAiming = false;
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
}