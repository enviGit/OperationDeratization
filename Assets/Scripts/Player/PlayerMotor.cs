using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float gravity = -9.8f;
    public float jumpHeight = 0.7f;
    private PlayerStance currentState = new PlayerStance();
    public float moveSpeed = 4f;
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 3f;
    public float ySensitivity = 3f;
    private bool isCrouching;
    private bool isShooting = false;
    private float shotTimer = 0f;
    private Gun currentWeapon;
    private bool isReloading = false;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject impactRicochet;
    public AudioSource gunAudio;
    private Gun weaponReload;
    private Gun previousWeapon;
    public GameObject crosshair;
    public bool isAiming = false;
    public bool isMoving = false;
    [SerializeField]
    private WeaponRecoil recoil;

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

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        Move();
        Gravity();
        Crouch();
        CrouchToggle();
        Jump();
        Shoot();
        Climb();
        PointerPosition();

        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.gunStyle != GunStyle.Melee && currentWeapon.magazineSize != currentWeapon.currentAmmoCount && currentWeapon.maxAmmoCount != 0)
        {
            weaponReload = currentWeapon;
            StartCoroutine(ReloadCoroutine());
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

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    private void Gravity()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            RaycastHit hit;
            LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

            if (Physics.Raycast(transform.position, transform.up, out hit, controller.height, obstacleMask))
                return;

            currentState.playerStance = PlayerStance.Stance.Idle;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
        if (Input.GetMouseButtonDown(0))
            isShooting = true;
        else if (Input.GetMouseButtonUp(0))
            isShooting = false;

        if (isShooting && Time.time > shotTimer && currentWeapon.currentAmmoCount == 0 && !isReloading)
        {
            isShooting = false;
            gunAudio.clip = currentWeapon.gunAudioClips[1];
            gunAudio.Play();
            return;
        }
        if (isShooting && Time.time > shotTimer && currentWeapon.currentAmmoCount > 0 && !isReloading)
        {
            RaycastHit hit;
            LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

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
                        GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);
                        GameObject ricochet = Instantiate(impactRicochet, hit.point, impactRotation);

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
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        gunAudio.clip = weaponReload.gunAudioClips[2];
        gunAudio.Play();
        yield return new WaitForSeconds(1.5f);

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
        Vector3 originalPosition;
        Vector3 originalRotation;
        Vector3 aimingPosition;
        Vector3 aimingRotation;

        switch (currentWeapon.gunType)
        {
            case GunType.Melee:
                originalPosition = new Vector3(0.05f, -0.08f, 0.2f);
                originalRotation = new Vector3(5.2f, -125, 101);
                aimingPosition = new Vector3(0.05f, -0.08f, 0.2f);
                aimingRotation = new Vector3(5.2f, -125, 101);
                break;
            case GunType.Pistol:
                originalPosition = new Vector3(0.16f, -0.25f, 0.5f);
                originalRotation = new Vector3(3f, 0, 0);
                aimingPosition = new Vector3(0, -0.1f, 0.24f);
                aimingRotation = new Vector3(0, 0, 0);
                break;
            default:
                originalPosition = new Vector3(0.16f, -0.25f, 0.5f);
                originalRotation = new Vector3(3f, 0, 0);
                aimingPosition = new Vector3(0, -0.17f, 0.24f);
                aimingRotation = new Vector3(0, 0, 0);
                break;
        }

        if (Input.GetMouseButton(1) && currentWeapon.gunStyle != GunStyle.Melee)
        {
            crosshair.gameObject.SetActive(true);
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
            crosshair.gameObject.SetActive(false);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, Time.deltaTime * 5f);
            isAiming = false;
            weapon.localPosition = originalPosition;
            weapon.localRotation = Quaternion.Euler(originalRotation);
            if (currentState.playerStance == PlayerStance.Stance.Idle || currentState.playerStance == PlayerStance.Stance.Walking)
                moveSpeed = 4f;
            else
                moveSpeed = 2f;
        }
    }
}