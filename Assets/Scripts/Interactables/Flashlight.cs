using UnityEngine;

public class Flashlight : MonoBehaviour
{
    Light flashlight;
    public PlayerHealth playerHealth;

    private void Start()
    {
        flashlight = GetComponent<Light>();
    }
    private void Update()
    {
        if (playerHealth.isAlive)
        {
            if (Input.GetKeyDown(KeyCode.T))
                flashlight.enabled = !flashlight.enabled;
        }
        else
            gameObject.SetActive(false);
    }
}