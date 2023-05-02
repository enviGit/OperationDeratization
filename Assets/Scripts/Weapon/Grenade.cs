using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("References")]
    public GameObject explosionEffect;
    public Gun grenade;
    private AudioSource bang;

    [Header("Grenade")]
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;
    public bool shouldExplode = false;
    bool hasExploded = false;
    float countdown;
    private float maxDistance = 50f;
    private float distance;
    private float volume;

    private void Start()
    {
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

        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        volume = Mathf.Clamp(1 - (distance / maxDistance), 0, 1);
    }
    private void Explode()
    {
        bang.clip = bang.GetComponent<ProjectileSound>().audioClips[0];
        bang.volume = volume;

        if (bang.clip != null)
            bang.Play();

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
                    grenade.countdown = 0.1f;
                    grenade.shouldExplode = true;

                    if(distance < radius)
                        foreach(Gun weapon in FindObjectOfType<PlayerInventory>().weapons)
                            if(weapon != null && weapon.gunStyle == GunStyle.Grenade)
                                FindObjectOfType<PlayerInventory>().CurrentWeapon.currentAmmoCount = 0;
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
            HitBox hitBox = nearbyObject.GetComponent<HitBox>();
            float distance = Vector3.Distance(nearbyObject.transform.position, transform.position);
            float damageRatio = Mathf.Clamp01(1f - (distance / radius));
            float damage = grenade.minimumDamage + (damageRatio * (grenade.maximumDamage - grenade.minimumDamage));
            int damageInt = Mathf.RoundToInt(damage);

            if (hitBox != null)
                hitBox.OnRaycastHit(grenade, new Vector3(0, 0, 0));
            else if (hitBox == null && nearbyObject.CompareTag("Player"))
                FindObjectOfType<PlayerHealth>().TakeDamage(damageInt);
        }

        Destroy(gameObject);
    }
}
