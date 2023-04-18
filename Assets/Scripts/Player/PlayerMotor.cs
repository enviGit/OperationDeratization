using TMPro;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float gravity = -9.8f;
    public float jumpHeight = 0.7f;
    private PlayerStance currentState = new PlayerStance();
    public float moveSpeed = 5f;
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 3f;
    public float ySensitivity = 3f;
    private bool isCrouching;
    private Animator anim;
    private bool isShooting = false;
    private float shotTimer = 0f;
    private Gun currentWeapon;
    [SerializeField]
    private TextMeshProUGUI ammoText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        currentState = new PlayerStance();
        currentState.playerStance = PlayerStance.Stance.Idle;
        currentState.camHeight = 3.4f;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //Debug.Log(currentState.playerStance);

        currentWeapon = GetComponent<PlayerInventory>().CurrentWeapon;
        isGrounded = controller.isGrounded;
        Move();

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        Gravity();
        Crouch();
        CrouchToggle();
        Jump();
        Shoot();
        Climb();
        PointerPosition();
        Reload();

        if (currentWeapon.gunStyle != GunStyle.Melee)
            UpdateAmmoText();
        else
            ammoText.text = "";
    }
    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        if (moveDirection.magnitude > 0)
        {
            if (isCrouching)
            {
                anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
                currentState.playerStance = PlayerStance.Stance.Crouching;
            }
            else
            {
                anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
                currentState.playerStance = PlayerStance.Stance.Walking;
            }
        }
        else
        {
            if (isCrouching)
            {
                anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
                currentState.playerStance = PlayerStance.Stance.Crouching;
            }
            else
            {
                anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
                currentState.playerStance = PlayerStance.Stance.Idle;
            }  
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
            float camNewHeight = Mathf.Lerp(Camera.main.transform.localPosition.y, 1.7f, Time.deltaTime * 5f);
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, camNewHeight, Camera.main.transform.localPosition.z);
            controller.height = 1f;
            moveSpeed = 2f;
        }
        else
        {
            float camNewHeight = Mathf.Lerp(Camera.main.transform.localPosition.y, 3.4f, Time.deltaTime * 5f);
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, camNewHeight, Camera.main.transform.localPosition.z);
            controller.height = 2f;
            moveSpeed = 5f;
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
                    anim.SetBool("IsCrouching", isCrouching);
                    currentState.playerStance = PlayerStance.Stance.Crouching;
                    currentState.camHeight = 1.7f;
                }
                else
                {
                    isCrouching = false;
                    anim.SetBool("IsCrouching", isCrouching);
                    currentState.playerStance = PlayerStance.Stance.Idle;
                    currentState.camHeight = 3.4f;
                }
            }
            else
            {
                isCrouching = true;
                anim.SetBool("IsCrouching", isCrouching);
                currentState.playerStance = PlayerStance.Stance.Crouching;
                currentState.camHeight = 1.7f;
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

        if (isShooting && Time.time > shotTimer && currentWeapon.currentAmmoCount > 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, currentWeapon.range))
            {
                if(currentWeapon.gunStyle != GunStyle.Melee)
                    currentWeapon.currentAmmoCount--;

                Debug.Log("Hit: " + hit.collider.name);
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();

                if (currentWeapon.gunStyle == GunStyle.Primary || currentWeapon.gunStyle == GunStyle.Secondary)
                {
                    Transform muzzle = transform.Find("Main Camera/WeaponHolder/" + currentWeapon.gunPrefab.name + "(Clone)/muzzle");
                    GameObject bullet = Instantiate(currentWeapon.bulletPrefab, muzzle.position + muzzle.forward * 0.5f, muzzle.rotation);
                    Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
                    bulletRigidbody.AddForce(muzzle.forward * currentWeapon.range, ForceMode.Impulse);
                }
                if (damageable != null)
                    damageable.DealDamage(Random.Range(currentWeapon.minimumDamage, currentWeapon.maximumDamage));
            }

            shotTimer = Time.time + currentWeapon.timeBetweenShots;
        }
    }
    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentWeapon.currentAmmoCount == currentWeapon.magazineSize)
                return;

            int ammoNeeded = currentWeapon.magazineSize - currentWeapon.currentAmmoCount;
            int ammoAvailable = Mathf.Min(currentWeapon.maxAmmoCount, ammoNeeded);

            if (ammoAvailable == 0)
                return;

            currentWeapon.currentAmmoCount += ammoAvailable;
            currentWeapon.maxAmmoCount -= currentWeapon.magazineSize;
        }  
    }
    private void UpdateAmmoText()
    {
        ammoText.text = currentWeapon.currentAmmoCount + " / " + currentWeapon.maxAmmoCount;
    }
    private void PointerPosition()
    {
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(0f, mouseX, 0f) * transform.localRotation;
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}