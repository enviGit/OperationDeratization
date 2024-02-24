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
        private GameObject mesh;
        private GameObject indicator;

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
            bang = GetComponent<AudioSource>();
            mesh = transform.GetChild(1).gameObject;
            indicator = transform.GetChild(2).gameObject;
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
            mesh.SetActive(false);
            indicator.SetActive(false);
            bang.Play();
            float delayBeforeDestroy = bang.clip.length;
            Invoke("DestroyObject", delayBeforeDestroy);
            ObjectPoolManager.SpawnObject(smokeEffect, transform.position, transform.rotation, ObjectPoolManager.PoolType.VFX);
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