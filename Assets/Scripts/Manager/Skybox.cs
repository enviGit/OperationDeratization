using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class Skybox : MonoBehaviour
    {
        [Header("Skybox Settings")]
        [SerializeField] private Material skybox;
        private float elapsedTime = 0f;
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");

        private void Update()
        {
            Move();
        }
        private void Move()
        {
            if (Time.timeScale != 0f)
            {
                elapsedTime += Time.deltaTime;
                float cloudsRotationSpeed = 0.5f;
                skybox.SetFloat(Rotation, elapsedTime * cloudsRotationSpeed);
            }
        }
        private void OnDisable()
        {
            skybox.SetFloat(Rotation, 0f);
        }
    }
}