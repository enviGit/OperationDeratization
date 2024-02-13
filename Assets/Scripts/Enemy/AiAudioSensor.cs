using RatGamesStudios.OperationDeratization.Manager;
using System;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class AiAudioSensor : MonoBehaviour
    {
        [SerializeField] private bool debugMode = true;
        private AiAgent agent;
        private Vector3 lastDetectedSoundPosition;
        [HideInInspector] public Vector3 LastDetectedSoundPosition => lastDetectedSoundPosition;
        private bool lastDetectedSoundAudible;
        [HideInInspector] public bool LastDetectedSoundAudible => lastDetectedSoundAudible;
        private readonly string[] highPrioritySounds = { "WeaponFire", "WeaponReload", "Impact", "Movement", "Grenade_00(Clone)", "ShatteredGlassPanel(Clone)" };

        private void HandleAudioEvent(AudioSource audioGameObject)
        {
            if (!agent.health.isAlive)
                return;
            if (!IsChildOfMyObject(audioGameObject.transform) && IsAudioAudible(audioGameObject))
            {
                if (debugMode && Time.timeScale != 0)
                {
                    string soundPath = GetGameObjectPath(audioGameObject.gameObject);
                    Debug.Log("Bot: " + gameObject.name + " has detected sound: " + audioGameObject.name + " at path: " + soundPath);
                } 
                if (IsHighPrioritySound(audioGameObject.name) && agent.weapons.HasWeapon() && !agent.targeting.HasTarget && !agent.weapons.IsLowAmmo())
                {
                    lastDetectedSoundPosition = audioGameObject.transform.position;
                    lastDetectedSoundAudible = true;
                    agent.stateMachine.ChangeState(AiStateId.InvestigateSound);
                }
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
        private bool IsHighPrioritySound(string soundName)
        {
            return Array.Exists(highPrioritySounds, sound => sound == soundName);
        }
        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;

            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }

            return path;
        }
        private void Start()
        {
            agent = GetComponent<AiAgent>();
            AudioEventManager.Instance.OnAudioEvent += HandleAudioEvent;
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