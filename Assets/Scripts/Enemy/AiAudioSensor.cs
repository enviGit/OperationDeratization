using RatGamesStudios.OperationDeratization.Manager;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class AiAudioSensor : MonoBehaviour
    {
        public bool debugMode = true;

        private void HandleAudioEvent(AudioSource audioGameObject)
        {
            if (!IsChildOfMyObject(audioGameObject.transform) && IsAudioAudible(audioGameObject))
            {
                if (debugMode)
                    Debug.Log("Bot: " + gameObject.name + " has detected sound: " + audioGameObject.name);
            }
        }
        private bool IsAudioAudible(AudioSource audioSource)
        {
            float distance = Vector3.Distance(transform.position, audioSource.transform.position);

            return distance <= audioSource.maxDistance && audioSource.spatialBlend == 1f;
        }
        private bool IsChildOfMyObject(Transform potentialChild)
        {
            if (potentialChild.parent != null)
                return potentialChild.parent.IsChildOf(transform);
            else
                return false;
        }
        private void Start()
        {
            AudioEventManager.Instance.OnAudioEvent += HandleAudioEvent;
        }
        private void OnDestroy()
        {
            AudioEventManager.Instance.OnAudioEvent -= HandleAudioEvent;
        }
        private void OnDrawGizmosSelected()
        {
            if (debugMode)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(transform.position, 30f);
            }
        }
    }
}