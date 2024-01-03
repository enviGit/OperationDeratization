using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Optimization.ObjectPooling
{
    public class ReturnToPoolAfterTimer : MonoBehaviour
    {
        public float timeToDespawn = 1f;
        private Coroutine _timerCoroutine;

        private void OnEnable()
        {
            _timerCoroutine = StartCoroutine(ReturnToPoolAfterTime());
        }
        private IEnumerator ReturnToPoolAfterTime()
        {
            float elapsedTime = 0f;

            while(elapsedTime < timeToDespawn)
            {
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}