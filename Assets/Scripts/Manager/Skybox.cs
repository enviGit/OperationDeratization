using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class Skybox : MonoBehaviour
    {
        [Header("Skybox Settings")]
        [SerializeField] private Material skybox;
        private float elapsedTime = 0f;
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");
        private static readonly int Exposure = Shader.PropertyToID("_Exposure");

        [Header("Sun Settings")]
        [SerializeField] private Light sun;
        [SerializeField] private float dayDuration = 300f;

        private void Update()
        {
            if (Time.timeScale != 0f)
            {
                elapsedTime += Time.deltaTime;
                float cloudsRotationSpeed = 0.4f;
                skybox.SetFloat(Rotation, elapsedTime * cloudsRotationSpeed);
                float normalizedTime = elapsedTime / dayDuration;
                float targetExposure = Mathf.Lerp(0.3f, 1f, Mathf.Sin(normalizedTime * Mathf.PI * 2) * 0.5f + 0.5f);
                skybox.SetFloat(Exposure, targetExposure);
                float sunRotationSpeed = 0.4f;
                sun.transform.rotation = Quaternion.Euler(130f, normalizedTime * 360f * sunRotationSpeed, 0f);
                float intensity = Mathf.Lerp(0.25f, 0.5f, targetExposure);
                sun.intensity = intensity;
            }
        }
    }
}