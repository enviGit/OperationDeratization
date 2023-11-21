using UnityEngine;

public class Smoke : MonoBehaviour
{
    [Header("References")]
    public GameObject smokeEffect;
    private AudioSource bang;
    private GameObject parentObject;

    [Header("Grenade")]
    public float delay = 2f;
    public bool shouldSmoke = false;
    bool hasSmoked = false;
    float countdown;

    [Header("Distance")]
    private float maxDistance = 20f;
    private float volume;

    private void Start()
    {
        parentObject = GameObject.Find("3D");
        countdown = delay;
        bang = GameObject.FindGameObjectWithTag("Bang").GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (shouldSmoke)
        {
            countdown -= Time.deltaTime;

            if (countdown <= 0 && !hasSmoked)
            {
                SmokeOn();
                hasSmoked = true;
            }
        }

        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        volume = Mathf.Clamp(1 - (distance / maxDistance), 0, 1);
    }
    private void SmokeOn()
    {
        bang.clip = bang.GetComponent<ProjectileSound>().audioClips[2];
        bang.volume = volume;

        if (bang.clip != null)
            bang.Play();

        Instantiate(smokeEffect, transform.position, transform.rotation, parentObject.transform);
        Destroy(gameObject);
    }
}
