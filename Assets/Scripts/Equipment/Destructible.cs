using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class Destructible : MonoBehaviour
    {
        public GameObject destroyedVersion;

        public void Destroy()
        {
            if (destroyedVersion != null)
                ObjectPoolManager.SpawnObject(destroyedVersion, transform.position, transform.rotation, ObjectPoolManager.PoolType.Gameobject);
            //Instantiate(destroyedVersion, transform.position, transform.rotation);

            ObjectPoolManager.ReturnObjectToPool(gameObject);
            //Destroy(gameObject);
        }
        /*public void DestroyFromBullet()
        {
            if (destroyedVersion != null)
                ObjectPoolManager.SpawnObject(destroyedVersion, transform.position, transform.rotation, ObjectPoolManager.PoolType.Gameobject);
            //Instantiate(destroyedVersion, transform.position, transform.rotation);

            ObjectPoolManager.ReturnObjectToPool(gameObject);
            //Destroy(gameObject);
        }*/
    }
}