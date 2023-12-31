using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class Destructible : MonoBehaviour
    {
        public GameObject destroyedVersion;

        public void Destroy()
        {
            if (destroyedVersion != null)
                Instantiate(destroyedVersion, transform.position, transform.rotation);

            Destroy(gameObject);
        }
        public void DestroyFromBullet()
        {
            if (destroyedVersion != null)
                Instantiate(destroyedVersion, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}