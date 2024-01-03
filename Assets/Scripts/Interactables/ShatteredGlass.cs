using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class ShatteredGlass : MonoBehaviour
    {
        [SerializeField] private float explosiveRadius = 0.5f;
        [SerializeField] private float destructionDelay = 3f;

        public void ApplyForce(Vector3 projectilePosition, float explosiveForce)
        {
            Collider[] colliders = Physics.OverlapSphere(projectilePosition, explosiveRadius);

            foreach(Collider collider in colliders)
                if(collider.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddExplosionForce(explosiveForce, projectilePosition, explosiveRadius);

            Invoke("DestroyObject", destructionDelay);
        }
        public void ApplyForceFromGrenade(Vector3 projectilePosition, float explosiveForce)
        {
            Collider[] colliders = Physics.OverlapSphere(projectilePosition, explosiveRadius * 4);

            foreach (Collider collider in colliders)
                if (collider.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddExplosionForce(explosiveForce, projectilePosition, explosiveRadius * 4);

            Invoke("DestroyObject", destructionDelay);
        }
        private void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}