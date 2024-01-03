using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerFlashlight : MonoBehaviour
    {
        [SerializeField] private Light flashlight;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] Image flashlightImg;
        [SerializeField] Sprite onImg;
        [SerializeField] Sprite offImg;

        private void Update()
        {
            if (playerHealth.isAlive)
            {
                if (Input.GetKeyDown(KeyCode.T))
                    flashlight.enabled = !flashlight.enabled;
                if (flashlight.enabled)
                {
                    flashlightImg.sprite = onImg;
                    float targetIntensity;

                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 5f, layerMask))
                    {
                        if (hit.distance <= 2.5f && hit.distance > 1f)
                            targetIntensity = 20f;
                        else if (hit.distance <= 1f)
                            targetIntensity = 3f;
                        else
                            targetIntensity = 50f;
                    }
                    else
                        targetIntensity = 150f;

                    flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, Time.deltaTime);
                }
                else
                    flashlightImg.sprite = offImg;
            }
            else
                gameObject.SetActive(false);
        }
    }
}