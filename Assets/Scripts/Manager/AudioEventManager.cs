using System;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class AudioEventManager : MonoBehaviour
    {
        private static AudioEventManager instance;
        public static AudioEventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();

                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(AudioEventManager).Name;
                        instance = obj.AddComponent<AudioEventManager>();
                    }
                }

                return instance;
            }
        }
        public event Action<AudioSource> OnAudioEvent;

        public void NotifyAudioEvent(AudioSource audioSource)
        {
            if (OnAudioEvent != null)
                OnAudioEvent(audioSource);
        }
    }
}