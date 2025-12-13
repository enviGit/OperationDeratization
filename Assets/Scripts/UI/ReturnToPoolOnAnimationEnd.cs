using RatGamesStudios.OperationDeratization.Optimization;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI
{
    public class ReturnToPoolOnAnimationEnd : MonoBehaviour
    {
        public void DestroyParent()
        {
            GameObject parent = gameObject.transform.parent.gameObject;
            ObjectPoolManager.ReturnObjectToPool(parent);
        }
    }
}