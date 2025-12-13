using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Optimization
{
    public class ReturnParticlesToPool : MonoBehaviour
    {
        private void OnParticleSystemStopped()
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}