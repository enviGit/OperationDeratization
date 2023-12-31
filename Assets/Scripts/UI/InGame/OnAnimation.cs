using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class OnAnimation : MonoBehaviour
    {
        public void DestroyParent()
        {
            GameObject parent = gameObject.transform.parent.gameObject;
            Destroy(parent);
        }
    }
}