using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using RatGamesStudios.OperationDeratization.UI.InGame;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class Grenade : MonoBehaviour
    {
        [Header("References")]
        public GameObject explosionEffect;
        public Gun grenade;
        private AudioSource bang;
        private GameObject mesh;
        private GameObject indicator;
        private AudioEventManager audioEventManager;

        [Header("Grenade")]
        public float delay = 3f;
        public float radius = 6.5f;
        public float force = 300f;
        public bool shouldExplode = false;
        private bool hasExploded = false;
        private float countdown;
        private float minimalCollisionForceToBreakGlass = 10f;

        private void Start()
        {
            countdown = delay;
            bang = GetComponent<AudioSource>();
            mesh = transform.GetChild(1).gameObject;
            indicator = transform.GetChild(2).gameObject;
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
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
            mesh.SetActive(false);
            indicator.SetActive(false);
            bang.Play();
            audioEventManager.NotifyAudioEvent(bang);
            float delayBeforeDestroy = bang.clip.length;
            Invoke("DestroyObject", delayBeforeDestroy);
            ObjectPoolManager.SpawnObject(explosionEffect, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
            LayerMask obstacleMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Postprocessing") | 1 << LayerMask.NameToLayer("Interactable"));
            Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in collidersToMove)
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(force, transform.position, radius);
            }

            Collider[] glassColliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in glassColliders)
            {
                Glass glass = nearbyObject.GetComponent<Glass>();

                if (glass != null)
                    glass.BreakFromGrenade(transform.position, force);
            }

            /*Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in collidersToDestroy)
            {
                Destructible dest = nearbyObject.GetComponent<Destructible>();

                if (dest != null)
                    dest.Destroy();
            }*/

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
                    /*RaycastHit hit;

                    if (Physics.Raycast(transform.position, nearbyObject.transform.position - transform.position, out hit, radius, obstacleMask))
                    {
                        if (hit.collider != nearbyObject)
                            continue;
                    }*/
                    if (nearbyObject.CompareTag("Enemy"))
                    {
                        hitBox.OnExplosion(damageInt, transform.forward);
                        //ObjectPoolManager.SpawnObject(bloodSpread, hit.point, impactRotation, hit.collider.transform);
                    }
                    if (nearbyObject.CompareTag("Player"))
                    {
                        hitBox.OnExplosionPlayer(damageInt);
                        DISystem.CreateIndicator(this.transform);
                    }
                }
            }
        }
        private void DestroyObject()
        {
            Destroy(gameObject);
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Glass"))
            {
                Glass glass = other.gameObject.GetComponent<Glass>();

                if (glass != null)
                {
                    float collisionForce = other.impulse.magnitude;

                    if (collisionForce >= minimalCollisionForceToBreakGlass)
                        glass.BreakFromGrenade(transform.position, grenade.impactForce);
                }
            }
            else
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity *= 1f;
            }
        }
    }
}