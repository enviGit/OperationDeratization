using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Glass : MonoBehaviour
    {
        [SerializeField] private ShatteredGlass shatteredVersion;

        public void Break(Vector3 projectilePosition)
        {
            ShatteredGlass shatteredGlass = Instantiate(shatteredVersion, transform.position, transform.rotation);
            shatteredGlass.transform.localScale = transform.localScale;
            shatteredGlass.ApplyForce(projectilePosition);
            Destroy(gameObject);
        }
    }
}