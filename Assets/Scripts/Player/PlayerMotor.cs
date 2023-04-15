using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float gravity = -9.8f;
    public float jumpHeight = 0.7f;
    [SerializeField]
    private Gun currentGun;
    private PlayerStance currentState = new PlayerStance();
    public float moveSpeed = 5f;
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 3f;
    public float ySensitivity = 3f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        currentState = new PlayerStance();
        currentState.playerStance = PlayerStance.Stance.Idle;
        currentState.camHeight = 3.4f;
    }

    private void Update()
    {
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
    }
    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        /*if (moveDirection.magnitude > 0)
            currentState.playerStance = PlayerStance.Stance.Walking;
        else
            currentState.playerStance = PlayerStance.Stance.Idle;*/

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
                    currentState.playerStance = PlayerStance.Stance.Crouching;
                    currentState.camHeight = 1.7f;
                }
                else
                {
                    currentState.playerStance = PlayerStance.Stance.Idle;
                    currentState.camHeight = 3.4f;
                }
            }
            else
            {
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
            if (currentState.playerStance == PlayerStance.Stance.Jumping)
            {
                currentState.playerStance = PlayerStance.Stance.Idle;
                moveSpeed = 3f;
            }
            else
            {
                currentState.playerStance = PlayerStance.Stance.Jumping;
                moveSpeed = 5f;
            }

            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                Debug.Log("Hit: " + hit.collider.name);
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();

                if (damageable != null)
                    damageable.DealDamage(Random.Range(currentGun.minimumDamage, currentGun.maximumDamage));
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