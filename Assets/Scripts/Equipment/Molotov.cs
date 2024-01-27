using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class Molotov : MonoBehaviour
    {
        [Header("References")]
        public GameObject molotovFire;
        private AudioSource bang;
        private GameObject mesh;
        private GameObject indicator;

        [Header("Grenade")]
        public bool shouldExplode = false;
        private bool hasExploded = false;
        private float minimalCollisionForceToBreakGlass = 10f;
        private float impactForce = 100f;

        private void Start()
        {
            bang = GetComponent<AudioSource>();
            mesh = transform.GetChild(1).gameObject;
            indicator = transform.GetChild(2).gameObject;
        }
        private void Explode()
        {
            mesh.SetActive(false);
            indicator.SetActive(false);
            bang.Play();
            float delayBeforeDestroy = bang.clip.length;
            Invoke("DestroyObject", delayBeforeDestroy);
            ObjectPoolManager.SpawnObject(molotovFire, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
        }
        private void DestroyObject()
        {
            Destroy(gameObject);
        }
        private void OnCollisionEnter(Collision other)
        {
            if (hasExploded)
                return;
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
            if (shouldExplode)
            {
                Explode();
                hasExploded = true;
            }
        }
    }
}