using UnityEngine;

public class Smoke : MonoBehaviour
{
    [Header("References")]
    public GameObject smokeEffect;

    [Header("Grenade")]
    public float delay = 2f;
    public bool shouldSmoke = false;
    bool hasSmoked = false;
    float countdown;

    private void Start()
    {
        countdown = delay;
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
    }
    private void SmokeOn()
    {
        Instantiate(smokeEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
