using RatGamesStudios.OperationDeratization.Manager;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("References")]
        private CharacterController controller;
        private PlayerStance currentState = new PlayerStance();
        private PlayerStamina stamina;
        private PlayerHealth health;
        private PlayerShoot aiming;
        private Camera cam;
        private AudioEventManager audioEventManager;
        [SerializeField] private Transform feet;

        [Header("Movement")]
        private Vector3 playerVelocity;
        public float gravity = -9.8f;
        public float jumpHeight = 0.7f;
        private float playerCrouchHeight = 2f;
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
        public bool _isClimbing = false;
        public bool isCrouching = false;
        public bool isMoving = false;
        public bool isRunning = false;
        private bool _isAiming = false;
        public bool shouldDetachFromLadder = false;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            controller = GetComponent<CharacterController>();
            currentState = new PlayerStance();
            currentState.playerStance = PlayerStance.Stance.Idle;
            cam = Camera.main;
            currentState.camHeight = 2f;
            stamina = GetComponent<PlayerStamina>();
            health = GetComponent<PlayerHealth>();
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
            aiming = GetComponent<PlayerShoot>();
            movementSound = transform.Find("Sounds/Movement").GetComponent<AudioSource>();
        }
        private void Update()
        {
            isGrounded = controller.isGrounded;
            _isAiming = aiming.isAiming;
            Jump();

            if (controller.enabled)
            {
                Move();
                CrouchToggle();
                Crouch();
                Gravity();
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
                {
                    currentState.playerStance = PlayerStance.Stance.Crouching;

                    if (!_isAiming)
                        moveSpeed = 2f;
                    else
                        moveSpeed = 1f;

                    movementSound.pitch = Random.Range(0.35f, 0.65f);
                    movementSound.volume = 0.5f;
                    movementSound.maxDistance = 5f;
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
                            movementSound.pitch = Random.Range(1.15f, 1.45f);
                            movementSound.volume = 1f;
                            movementSound.maxDistance = 35f;
                            movementSound.clip = movementClips[1];

                        }
                        else
                        {
                            movementSound.pitch = Random.Range(0.85f, 1.15f);
                            movementSound.volume = 0.9f;
                            movementSound.maxDistance = 30f;
                            movementSound.clip = movementClips[0];
                        }
                    }
                }
                if (!movementSound.isPlaying)
                {
                    movementSound.Play();
                    audioEventManager.NotifyAudioEvent(movementSound);
                } 
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
            if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isCrouching && stamina.currentStamina > 0f && !_isAiming)
            {
                isRunning = true;
                moveSpeed *= 1.3f;
            }
            else
                isRunning = false;

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
                    float fallDamage = fallTime * fallDamageMultiplier * Time.timeScale;

                    if (fallDamage > 0)
                        fallDamageTaken += fallDamage;
                }
            }
        }
        private void Crouch()
        {
            if (currentState.playerStance == PlayerStance.Stance.Crouching)
            {
                float camNewHeight = Mathf.Lerp(cam.transform.localPosition.y, currentState.camHeight, Time.deltaTime * 5f);
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, camNewHeight, cam.transform.localPosition.z);
                controller.height = 2.5f;
            }
            else
            {
                float camNewHeight = Mathf.Lerp(cam.transform.localPosition.y, currentState.camHeight, Time.deltaTime * 5f);
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, camNewHeight, cam.transform.localPosition.z);
                controller.height = 3.6f;
            }
        }
        private void CrouchToggle()
        {
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (!isGrounded)
                    return;
                if (currentState.playerStance == PlayerStance.Stance.Crouching)
                {
                    RaycastHit hit;
                    LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Postprocessing"));

                    if (Physics.Raycast(feet.position, transform.up, out hit, playerCrouchHeight, obstacleMask))
                    {
                        isCrouching = true;
                        currentState.playerStance = PlayerStance.Stance.Crouching;
                        currentState.camHeight = 1.5f;
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
                    currentState.camHeight = 1.5f;
                }
            }
        }
        private void Jump()
        {
            if (Input.GetKey(KeyCode.Space) && isGrounded && stamina.HasStamina(stamina.jumpStaminaCost / 2))
            {
                RaycastHit hit;
                LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Postprocessing"));

                if (!isCrouching && Physics.Raycast(feet.position, transform.up, out hit, jumpHeight, obstacleMask))
                    return;
                else if (isCrouching && Physics.Raycast(feet.position, transform.up, out hit, playerCrouchHeight + jumpHeight, obstacleMask))
                    return;
                if (_isClimbing)
                    shouldDetachFromLadder = true;
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
                audioEventManager.NotifyAudioEvent(movementSound);
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
}