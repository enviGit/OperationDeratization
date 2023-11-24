using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private PlayerStance currentState = new PlayerStance();
    private LadderTrigger ladder;
    private PlayerStamina stamina;
    private PlayerHealth health;
    private PlayerShoot aiming;

    [Header("Movement")]
    private Vector3 playerVelocity;
    public float gravity = -9.8f;
    public float jumpHeight = 0.7f;
    public float moveSpeed = 4f;
    private AudioSource movementSound;
    public AudioClip[] movementClips;

    [Header("Fall damage")]
    public float fallDamageMultiplier = 1.5f;
    private float fallTime = 0f;
    private float fallDamageTaken;
    private float fallTimeCalc = 0.7f;

    [Header("Bool checks")]
    public bool isGrounded;
    private bool _isClimbing = false;
    public bool isCrouching = false;
    public bool isMoving = false;
    public bool isRunning = false;
    private bool _isAiming = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        currentState = new PlayerStance();
        currentState.playerStance = PlayerStance.Stance.Idle;
        currentState.camHeight = 2f;
        stamina = GetComponent<PlayerStamina>();
        health = GetComponent<PlayerHealth>();
        ladder = FindObjectOfType<LadderTrigger>();
        aiming = GetComponent<PlayerShoot>();
        movementSound = transform.Find("Sounds/Movement").GetComponent<AudioSource>();
    }
    private void Update()
    {
        isGrounded = controller.isGrounded;
        _isAiming = aiming.isAiming;

        if (ladder != null)
            _isClimbing = ladder.isClimbing;

        Move();
        Crouch();
        CrouchToggle();
        Jump();
        Gravity();
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
            {
                currentState.playerStance = PlayerStance.Stance.Crouching;

                if (!_isAiming)
                    moveSpeed = 2f;
                else
                    moveSpeed = 1f;
                
                movementSound.pitch = 0.5f;
                movementSound.clip = movementClips[0];
            }
            else
            {
                currentState.playerStance = isRunning ? PlayerStance.Stance.Running : PlayerStance.Stance.Walking;

                if (!_isAiming)
                    moveSpeed = 4f;
                else
                    moveSpeed = 2f;
                if (isGrounded)
                {
                    if (isRunning)
                    {
                        movementSound.pitch = 1.3f;
                        movementSound.clip = movementClips[1];

                    }
                    else
                    {
                        movementSound.pitch = 1f;
                        movementSound.clip = movementClips[0];
                    }
                }
            }
            if (!movementSound.isPlaying)
                movementSound.Play();
        }
        else
        {
            isMoving = false;

            if (isCrouching)
                currentState.playerStance = PlayerStance.Stance.Crouching;
            else
                currentState.playerStance = PlayerStance.Stance.Idle;

            movementSound.Stop();
        }

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isCrouching && stamina.currentStamina > 0f)
        {
            isRunning = true;
            moveSpeed *= 1.3f;
        }
        else
        {
            isRunning = false;
            moveSpeed /= 1.3f;
        }

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void Gravity()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (_isClimbing)
                isGrounded = false;

            fallTime = 0f;
            playerVelocity.y = -2f;

            if (fallDamageTaken > 0)
            {
                //Debug.Log(fallDamageTaken);
                health.TakeFallingDamage(fallDamageTaken);
                fallDamageTaken = 0;
            }
        }
        else
        {
            if (!_isClimbing)
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
        }
        else
        {
            float camNewHeight = Mathf.Lerp(Camera.main.transform.localPosition.y, 2f, Time.deltaTime * 5f);
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, camNewHeight, Camera.main.transform.localPosition.z);
            controller.height = 2f;
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
        if (Input.GetKey(KeyCode.Space) && isGrounded && stamina.HasStamina(stamina.jumpStaminaCost / 2))
        {
            RaycastHit hit;
            LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player"));

            if (Physics.Raycast(transform.position, transform.up, out hit, controller.height, obstacleMask))
                return;
            if (_isClimbing)
                ladder.DetachFromLadder();
            if (isCrouching)
            {
                isCrouching = false;
                currentState.playerStance = PlayerStance.Stance.Idle;
                currentState.camHeight = 2f;
            }

            fallTimeCalc = 1.2f;
            fallDamageMultiplier = 0.6f;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            stamina.UseStamina(stamina.jumpStaminaCost);
            movementSound.clip = movementClips[2];
            movementSound.pitch = 1f;
            movementSound.Play();
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
}