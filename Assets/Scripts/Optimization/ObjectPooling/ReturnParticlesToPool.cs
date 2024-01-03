using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Optimization.ObjectPooling
{
    public class ReturnParticlesToPool : MonoBehaviour
    {
        private void OnParticleSystemStopped()
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}