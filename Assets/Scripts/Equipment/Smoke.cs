using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class Smoke : MonoBehaviour
    {
        [Header("References")]
        public GameObject smokeEffect;
        private AudioSource bang;

        [Header("Grenade")]
        public float delay = 2f;
        public bool shouldSmoke = false;
        private bool hasSmoked = false;
        private float countdown;
        private float minimalCollisionForceToBreakGlass = 10f;
        private float impactForce = 100f;

        private void Start()
        {
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
        }
        private void SmokeOn()
        {
            bang.PlayOneShot(bang.GetComponent<ProjectileSound>().audioClips[2]);
            ObjectPoolManager.SpawnObject(smokeEffect, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
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
                        glass.BreakFromGrenade(transform.position, impactForce);
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