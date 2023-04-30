using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("References")]
    public GameObject explosionEffect;

    [Header("Grenade")]
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;
    public bool shouldExplode = false;
    bool hasExploded = false;
    float countdown;

    private void Start()
    {
        countdown = delay;
    }
    private void Update()
    {
        if (shouldExplode)
        {
            countdown -= Time.deltaTime;

            if (countdown <= 0 && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }
    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(force, transform.position, radius);
        }

        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToDestroy)
        {
            Destructible dest = nearbyObject.GetComponent<Destructible>();

            if (dest != null)
                dest.Destroy();
        }

        Destroy(gameObject);
    }
}
