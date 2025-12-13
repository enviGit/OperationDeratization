using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerLeaning : MonoBehaviour
    {
        [Header("Settings")]
        public float leanAngle = 15f;
        public float leanSpeed = 10f;

        private float currentLean = 0f;
        private float targetLean = 0f;

        private void Update()
        {
            HandleInput();
            ApplyLean();
        }

        private void HandleInput()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                targetLean = leanAngle;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                targetLean = -leanAngle;
            }
            else
            {
                targetLean = 0f;
            }
        }

        private void ApplyLean()
        {
            currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);

            Vector3 targetEuler = transform.localRotation.eulerAngles;

            Quaternion newRotation = Quaternion.Euler(targetEuler.x, targetEuler.y, currentLean);

            transform.localRotation = newRotation;
        }
    }
}