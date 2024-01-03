using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
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