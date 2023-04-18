using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    private void FixedUpdate()
    {
        LookAtPlayer();
    }
    private void LookAtPlayer()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
