using RatGamesStudios.OperationDeratization.Optimization;
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

            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        /*public void DestroyFromBullet()
        {
            if (destroyedVersion != null)
                ObjectPoolManager.SpawnObject(destroyedVersion, transform.position, transform.rotation, ObjectPoolManager.PoolType.Gameobject);

            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }*/
    }
}