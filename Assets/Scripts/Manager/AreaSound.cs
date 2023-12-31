using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class AreaSound : MonoBehaviour
    {
        public GameObject gameManager;
        private AudioSource audioSource;
        private AudioClip defaultClip;
        public float fadeDuration = 1f;

        private void Start()
        {
            audioSource = gameManager.GetComponent<AudioSource>();
            defaultClip = audioSource.clip;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                string areaName = gameObject.name;

                switch (areaName)
                {
                    case "Global":
                        ChangeAudioClipWithFade("GlobalClip");
                        break;
                    case "CitySphere":
                        ChangeAudioClipWithFade("CityClip");
                        break;
                    case "GraveyardSphere":
                        ChangeAudioClipWithFade("GraveyardClip");
                        break;
                    case "FarmSphere":
                        ChangeAudioClipWithFade("FarmClip");
                        break;
                    default:
                        break;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                StartCoroutine(FadeToDefaultClip());
        }
        private void ChangeAudioClipWithFade(string clipName)
        {
            AudioClip newClip = Resources.Load<AudioClip>("Audio/Music/" + clipName);

            if (newClip != null)
                StartCoroutine(FadeOutAndChangeClip(newClip));
            else
                Debug.LogWarning("AudioClip " + clipName + " not found!");
        }
        private IEnumerator FadeOutAndChangeClip(AudioClip newClip)
        {
            float timer = 0f;
            float startVolume = audioSource.volume;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);

                yield return null;
            }

            audioSource.clip = newClip;
            audioSource.Play();
            timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, startVolume, timer / fadeDuration);

                yield return null;
            }
        }
        private IEnumerator FadeToDefaultClip()
        {
            float timer = 0f;
            float startVolume = audioSource.volume;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);

                yield return null;
            }

            audioSource.clip = defaultClip;
            audioSource.Play();

            timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, startVolume, timer / fadeDuration);

                yield return null;
            }
        }
    }
}