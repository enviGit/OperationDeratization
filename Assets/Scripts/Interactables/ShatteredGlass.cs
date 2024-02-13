using RatGamesStudios.OperationDeratization.Manager;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class ShatteredGlass : MonoBehaviour
    {
        [SerializeField] private float explosiveRadius = 0.5f;
        [SerializeField] private float destructionDelay = 3f;
        private AudioEventManager audioEventManager;
        private AudioSource sound;

        private void Awake()
        {
            sound = GetComponent<AudioSource>();
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
        }
        public void ApplyForce(Vector3 projectilePosition, float explosiveForce)
        {
            Collider[] colliders = Physics.OverlapSphere(projectilePosition, explosiveRadius);

            foreach(Collider collider in colliders)
                if(collider.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddExplosionForce(explosiveForce, projectilePosition, explosiveRadius);

            audioEventManager.NotifyAudioEvent(sound);
            Invoke("DestroyObject", destructionDelay);
        }
        public void ApplyForceFromGrenade(Vector3 projectilePosition, float explosiveForce)
        {
            Collider[] colliders = Physics.OverlapSphere(projectilePosition, explosiveRadius * 4);

            foreach (Collider collider in colliders)
                if (collider.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddExplosionForce(explosiveForce, projectilePosition, explosiveRadius * 4);

            audioEventManager.NotifyAudioEvent(sound);
            Invoke("DestroyObject", destructionDelay);
        }
        private void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}