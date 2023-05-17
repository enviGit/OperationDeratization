using UnityEngine;

public class Flashlight : MonoBehaviour
{
    Light flashlight;

    private void Start()
    {
        flashlight = GetComponent<Light>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            flashlight.enabled = !flashlight.enabled;
    }
}
