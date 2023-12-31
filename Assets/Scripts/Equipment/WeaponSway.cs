using RatGamesStudios.OperationDeratization.Player;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class WeaponSway : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController mover;
        [SerializeField] private Rigidbody rb;
        private PlayerMotor playerMotor;
        private PlayerShoot playerShoot;

        [Header("Sway and bob")]
        public float step = 0.01f;
        public float maxStepDistance = 0.06f;
        public float rotationStep = 4f;
        public float maxRotationStep = 5f;
        public float speedCurve;
        public Vector3 travelLimit = Vector3.one * 0.025f;
        public Vector3 bobLimit = Vector3.one * 0.01f;
        public Vector3 multiplier = Vector3.one * 1.5f;
        float smooth = 10f;
        float smoothRot = 12f;
        float curveSin { get => Mathf.Sin(speedCurve); }
        float curveCos { get => Mathf.Cos(speedCurve); }
        Vector2 walkInput;
        Vector2 lookInput;
        Vector3 swayPos;
        Vector3 swayEulerRot;
        Vector3 bobPosition;
        Vector3 bobEulerRotation;

        [Header("Bool checks")]
        private bool isAiming;
        private bool isMoving;
        private bool isRunning;

        private void Start()
        {
            playerMotor = FindObjectOfType<PlayerMotor>();
            playerShoot = FindObjectOfType<PlayerShoot>();
        }
        private void Update()
        {
            isAiming = playerShoot.isAiming;
            isMoving = playerMotor.isMoving;
            isRunning = playerMotor.isRunning;

            if (isRunning && !isAiming)
                multiplier = Vector3.one * 5f;
            else if (isMoving && !isAiming)
                multiplier = Vector3.one * 2.5f;
            else if (isMoving && isAiming)
                multiplier = Vector3.one * 0.3f;
            else if (!isMoving && !isAiming)
                multiplier = Vector3.one * 1f;
            else
                multiplier = Vector3.one * 0f;

            GetInput();
            Sway();
            SwayRotation();
            BobOffset();
            BobRotation();
            CompositePositionRotation();
        }
        private void GetInput()
        {
            walkInput.x = Input.GetAxisRaw("Horizontal");
            walkInput.y = Input.GetAxisRaw("Vertical");
            walkInput = walkInput.normalized;
            lookInput.x = Input.GetAxisRaw("Mouse X");
            lookInput.y = Input.GetAxisRaw("Mouse Y");
        }
        private void Sway()
        {
            Vector3 invertLook = lookInput * -step;
            invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
            invertLook.x = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);
            swayPos = invertLook;
        }
        private void SwayRotation()
        {
            Vector2 invertLook = lookInput * -rotationStep;
            invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
            invertLook.x = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
            swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
        }
        private void CompositePositionRotation()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos, Time.deltaTime * smooth);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
        }
        private void BobOffset()
        {
            speedCurve += Time.deltaTime * (mover.isGrounded ? rb.velocity.magnitude : 1f) + 0.01f;
            bobPosition.x = (curveCos * bobLimit.x * (mover.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
            bobPosition.y = (curveSin * bobLimit.y) - (rb.velocity.y * travelLimit.y);
            bobPosition.z = -(walkInput.y * travelLimit.z);
        }
        private void BobRotation()
        {
            bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
            bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
            bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
        }
    }
}