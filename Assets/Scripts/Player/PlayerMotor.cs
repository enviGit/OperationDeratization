using RatGamesStudios.OperationDeratization.Manager;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform feet;
        [SerializeField] private PlayerStamina stamina;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private PlayerShoot aiming;

        [SerializeField] private AudioSource movementSound;
        [SerializeField] private AudioClip[] movementClips;

        private CharacterController controller;
        private Camera cam;
        private AudioEventManager audioEventManager;

        [Header("Settings")]
        public float walkSpeed = 4f;
        public float runSpeed = 6f;
        public float crouchSpeed = 2f;
        public float aimSpeed = 2f;
        public float gravity = -9.81f;
        public float jumpHeight = 1.0f;

        [Header("Crouch Settings")]
        [SerializeField] private float standHeight = 3.6f;
        [SerializeField] private float crouchHeight = 2.5f;
        [SerializeField] private float standCamHeight = 2f;
        [SerializeField] private float crouchCamHeight = 1.5f;
        [SerializeField] private float crouchTransitionSpeed = 10f;

        [Header("Fall Damage")]
        public float fallDamageMultiplier = 5f;
        public float minFallVelocity = -15f;

        [Header("State")]
        public bool isGrounded;
        public bool isCrouching;
        public bool isMoving;
        public bool isRunning;
        public bool _isClimbing;
        public bool shouldDetachFromLadder;

        private Vector3 playerVelocity;
        [HideInInspector] public float currentSpeed;
        private LayerMask playerMask;

        private float stepCycle = 0f;
        private float nextStep = 0f;
        [SerializeField] private float stepInterval = 0.5f;
        [SerializeField] private float runStepInterval = 0.3f;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            cam = Camera.main;

            var audioMgr = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioMgr) audioEventManager = audioMgr.GetComponent<AudioEventManager>();

            if (stamina == null) stamina = GetComponent<PlayerStamina>();
            if (health == null) health = GetComponent<PlayerHealth>();
            if (aiming == null) aiming = GetComponent<PlayerShoot>();

            playerMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Postprocessing") | 1 << LayerMask.NameToLayer("Hitbox"));

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            isGrounded = controller.isGrounded;

            if (controller.enabled)
            {
                HandleMovement();
                HandleGravity();
                HandleJump();
                HandleCrouch();
            }
        }

        private void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 inputDir = transform.right * x + transform.forward * z;

            isMoving = inputDir.sqrMagnitude > 0.01f;

            if (isCrouching)
            {
                currentSpeed = crouchSpeed;
                isRunning = false;
            }
            else if (aiming.isAiming)
            {
                currentSpeed = aimSpeed;
                isRunning = false;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && isGrounded && stamina.currentStamina > 0)
            {
                isRunning = true;
                currentSpeed = runSpeed;
            }
            else
            {
                isRunning = false;
                currentSpeed = walkSpeed;
            }

            if (inputDir.magnitude > 1) inputDir.Normalize();

            controller.Move(inputDir * currentSpeed * Time.deltaTime);

            if (isMoving && isGrounded)
            {
                HandleFootsteps(currentSpeed);
            }
            else
            {
                stepCycle = 0f;
                nextStep = 0f;
            }
        }

        private void HandleFootsteps(float speed)
        {
            stepCycle += (speed * (isRunning ? 1f : 0.8f)) * Time.deltaTime;

            if (stepCycle > nextStep)
            {
                nextStep = stepCycle + (isRunning ? runStepInterval : stepInterval);
                PlayFootstepAudio();
            }
        }

        private void PlayFootstepAudio()
        {
            if (movementSound == null || movementClips.Length == 0) return;

            movementSound.pitch = Random.Range(0.85f, 1.1f);
            movementSound.volume = isCrouching ? 0.3f : (isRunning ? 1f : 0.6f);

            movementSound.PlayOneShot(isRunning && movementClips.Length > 1 ? movementClips[1] : movementClips[0]);

            audioEventManager?.NotifyAudioEvent(movementSound);
        }

        private void HandleGravity()
        {
            if (isGrounded && playerVelocity.y < 0)
            {
                if (playerVelocity.y < minFallVelocity)
                {
                    float damage = Mathf.Abs(playerVelocity.y + minFallVelocity) * fallDamageMultiplier;
                    health.TakeFallingDamage(damage);
                }
                playerVelocity.y = -2f;
            }

            if (!_isClimbing)
            {
                playerVelocity.y += gravity * Time.deltaTime;
            }
            else
            {
                playerVelocity.y = 0;
            }

            controller.Move(playerVelocity * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                if (isCrouching)
                {
                    if (CanStandUp())
                    {
                        isCrouching = false;
                    }
                    else return;
                }

                if (stamina.HasStamina(stamina.jumpStaminaCost))
                {
                    stamina.UseStamina(stamina.jumpStaminaCost);

                    playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                    if (_isClimbing) shouldDetachFromLadder = true;

                    if (movementClips.Length > 2)
                    {
                        movementSound.pitch = Random.Range(0.9f, 1.1f);
                        movementSound.PlayOneShot(movementClips[2]);
                        audioEventManager?.NotifyAudioEvent(movementSound);
                    }
                }
            }
        }

        private void HandleCrouch()
        {
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (isCrouching)
                {
                    if (CanStandUp()) isCrouching = false;
                }
                else
                {
                    isCrouching = true;
                }
            }

            float targetHeight = isCrouching ? crouchHeight : standHeight;
            float targetCamHeight = isCrouching ? crouchCamHeight : standCamHeight;

            controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

            controller.center = new Vector3(0, controller.height / 2f, 0);

            Vector3 camPos = cam.transform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCamHeight, Time.deltaTime * crouchTransitionSpeed);
            cam.transform.localPosition = camPos;
        }

        private bool CanStandUp()
        {
            return !Physics.Raycast(feet.position, Vector3.up, standHeight, playerMask);
        }
    }
}