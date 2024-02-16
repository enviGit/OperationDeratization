using RatGamesStudios.OperationDeratization.Player;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class WeaponSway : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController mover;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private PlayerMotor playerMotor;
        [SerializeField] private PlayerShoot playerShoot;

        [Header("Sway")]
        public float speedCurve;
        public Vector3 travelLimit = Vector3.one * 0.025f;
        public Vector3 swayLimit = Vector3.one * 0.01f;
        public Vector3 multiplier = Vector3.one * 1.5f;
        private float smoothRot = 12f;
        private float curveSin { get => Mathf.Sin(speedCurve); }
        private float curveCos { get => Mathf.Cos(speedCurve); }
        private Vector2 walkInput;
        private Vector3 swayPosition;
        private Vector3 swayEulerRotation;
        private float timeScale;

        [Header("Bool checks")]
        private bool isAiming;
        private bool isMoving;
        private bool isRunning;

        private void Update()
        {
            timeScale = Time.timeScale;
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
            SwayOffset();
            SwayRotation();
            CompositePositionRotation();
        }
        private void GetInput()
        {
            walkInput.x = Input.GetAxisRaw("Horizontal");
            walkInput.y = Input.GetAxisRaw("Vertical");
            walkInput = walkInput.normalized;
        }
        private void CompositePositionRotation()
        {
            float deltaTime = Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRotation), deltaTime * smoothRot * Time.timeScale);
        }
        private void SwayOffset()
        {
            float deltaTime = Time.deltaTime;
            float groundedMultiplier = mover.isGrounded ? 1 : 0;
            speedCurve += deltaTime * (mover.isGrounded ? rb.velocity.magnitude : 1f) * timeScale + 0.01f;
            swayPosition.x = (curveCos * swayLimit.x * groundedMultiplier) - (walkInput.x * travelLimit.x);
            swayPosition.y = (curveSin * swayLimit.y) - (rb.velocity.y * travelLimit.y);
            swayPosition.z = -(walkInput.y * travelLimit.z);
        }
        private void SwayRotation()
        {
            swayEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
            swayEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
            swayEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
        }
    }
}