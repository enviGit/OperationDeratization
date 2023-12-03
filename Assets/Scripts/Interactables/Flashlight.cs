using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    public PlayerHealth playerHealth;
    public LayerMask layerMask;

    private void Update()
    {
        if (playerHealth.isAlive)
        {
            if (Input.GetKeyDown(KeyCode.T))
                flashlight.enabled = !flashlight.enabled;
            if(flashlight.enabled)
            {
                float targetIntensity;

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 5f, layerMask))
                {
                    if (hit.distance <= 2.5f && hit.distance > 1f)
                        targetIntensity = 20f;
                    else if (hit.distance <= 1f)
                        targetIntensity = 10f;
                    else
                    targetIntensity = 50f;
                }    
                else
                    targetIntensity = 150f;

                flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, Time.deltaTime);
            }
            
        }
        else
            gameObject.SetActive(false);
    }
}