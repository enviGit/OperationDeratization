using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("References")]
    public GameObject explosionEffect;
    public Gun grenade;

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
            {
                rb.AddExplosionForce(force, transform.position, radius);
                Grenade grenade = nearbyObject.GetComponent<Grenade>();

                if (grenade != null)
                {
                    grenade.countdown = 1f;
                    grenade.shouldExplode = true;
                }
            }
        }

        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToDestroy)
        {
            Destructible dest = nearbyObject.GetComponent<Destructible>();

            if (dest != null)
                dest.Destroy();
        }

        Collider[] collidersToDamage = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToDamage)
        {
            IDamageable damageable = nearbyObject.GetComponent<IDamageable>();
            float distance = Vector3.Distance(nearbyObject.transform.position, transform.position);
            float damageRatio = Mathf.Clamp01(1f - (distance / radius));
            float damage = grenade.minimumDamage + (damageRatio * (grenade.maximumDamage - grenade.minimumDamage));
            int damageInt = Mathf.RoundToInt(damage);

            if (damageable != null)
                damageable.DealDamage(damageInt);
            else if (damageable == null && nearbyObject.CompareTag("Player"))
                FindObjectOfType<PlayerHealth>().TakeDamage(damageInt);
        }

        Destroy(gameObject);
    }
}
