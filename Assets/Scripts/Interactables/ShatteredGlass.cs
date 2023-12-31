using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class ShatteredGlass : MonoBehaviour
    {
        [SerializeField] private float explosiveForce = 150f;
        [SerializeField] private float explosiveRadius = 0.5f;
        [SerializeField] private float destructionDelay = 5f;

        public void ApplyForce(Vector3 projectilePosition)
        {
            Collider[] colliders = Physics.OverlapSphere(projectilePosition, explosiveRadius);

            foreach(Collider collider in colliders)
                if(collider.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddExplosionForce(explosiveForce, projectilePosition, explosiveRadius);

            Invoke("DestroyObject", destructionDelay);
        }
        private void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}