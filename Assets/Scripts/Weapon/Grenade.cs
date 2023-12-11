using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("References")]
    public GameObject explosionEffect;
    public Gun grenade;
    private AudioSource bang;
    private GameObject parentObject;

    [Header("Grenade")]
    public float delay = 3f;
    public float radius = 5f;
    public float force = 100f;
    public bool shouldExplode = false;
    bool hasExploded = false;
    float countdown;

    private void Start()
    {
        parentObject = GameObject.Find("3D");
        countdown = delay;
        bang = GameObject.FindGameObjectWithTag("Bang").GetComponent<AudioSource>();
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
    public void Explode()
    {
        bang.PlayOneShot(bang.GetComponent<ProjectileSound>().audioClips[0]);
        Instantiate(explosionEffect, transform.position, transform.rotation, parentObject.transform);
        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if (rb != null)
                if (!nearbyObject.CompareTag("Enemy"))
                    rb.AddExplosionForce(force, transform.position, radius);

            Grenade grenade = nearbyObject.GetComponent<Grenade>();

            if (grenade != null)
            {
                grenade.countdown = 0.1f;
                grenade.shouldExplode = true;
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
            HitBox hitBox = nearbyObject.GetComponent<HitBox>();
            float distance = Vector3.Distance(nearbyObject.transform.position, transform.position);
            float damageRatio = Mathf.Clamp01(1f - (distance / radius));
            float damage = grenade.minimumDamage + (damageRatio * (grenade.maximumDamage - grenade.minimumDamage));
            int damageInt = Mathf.RoundToInt(damage);

            if (hitBox != null)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, nearbyObject.transform.position - transform.position, out hit, radius))
                {
                    if (hit.collider != nearbyObject)
                        continue;
                }
                if (nearbyObject.CompareTag("Enemy"))
                    hitBox.OnExplosion(damageInt, transform.forward);
                if (nearbyObject.CompareTag("Player"))
                {
                    hitBox.OnExplosionPlayer(damageInt);
                    DISystem.CreateIndicator(this.transform);
                }
            }
        }

        Destroy(gameObject);
    }
}
