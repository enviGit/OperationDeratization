using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Optimization.ObjectPooling
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
        private GameObject objectPoolEmptyHolder;
        private static GameObject particleSystemsEmpty;
        private static GameObject gameObjectsEmpty;
        public enum PoolType
        {
            ParticleSystem,
            Gameobject
        }
        public static PoolType PoolingType;

        private void Awake()
        {
            SetupEmpties();
        }
        private void SetupEmpties()
        {
            objectPoolEmptyHolder = new GameObject("///Pooled Objects");
            particleSystemsEmpty = new GameObject("Particle Effects");
            particleSystemsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);
            gameObjectsEmpty = new GameObject("Game Objects");
            gameObjectsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);
        }
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform parent = null)
        {
            PooledObjectInfo pool = null;

            foreach (PooledObjectInfo p in ObjectPools)
            {
                if (p.LookupString == objectToSpawn.name)
                {
                    pool = p;

                    break;
                }
            }

            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null)
            {
                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

                if (parent != null)
                    spawnableObj.transform.SetParent(parent);
            }
            else
            {
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;

                if (parent != null)
                    spawnableObj.transform.SetParent(parent);

                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType)
        {
            PooledObjectInfo pool = null;

            foreach (PooledObjectInfo p in ObjectPools)
            {
                if (p.LookupString == objectToSpawn.name)
                {
                    pool = p;

                    break;
                }
            }

            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null)
            {
                GameObject parentObject = SetParentObject(poolType);
                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
                spawnableObj.transform.SetParent(parentObject.transform);
            }
            else
            {
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }
        public static void ReturnObjectToPool(GameObject obj)
        {
            string objName = obj.name.Substring(0, obj.name.Length - 7);
            PooledObjectInfo pool = null;

            foreach (PooledObjectInfo p in ObjectPools)
            {
                if (p.LookupString == objName)
                {
                    pool = p;

                    break;
                }
            }

            if (pool == null)
                Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
            else
            {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }
        private static GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.ParticleSystem:
                    return particleSystemsEmpty;
                case PoolType.Gameobject:
                    return gameObjectsEmpty;
                default:
                    return null;
            }
        }
    }
    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
    }
}