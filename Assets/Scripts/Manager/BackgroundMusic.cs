using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class BackgroundMusic : MonoBehaviour
    {
        private AudioSource audioSource;
        private AudioClip[] musicClips;
        private int currentClipIndex = -1;
        public float fadeDuration = 1f;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            LoadMusicClips();
            PlayRandomMusic();
        }
        private void LoadMusicClips()
        {
            Object[] clips = Resources.LoadAll("Audio/Music", typeof(AudioClip));
            musicClips = new AudioClip[clips.Length];

            for (int i = 0; i < clips.Length; i++)
                musicClips[i] = (AudioClip)clips[i];
        }
        private void PlayRandomMusic()
        {
            if (musicClips == null || musicClips.Length == 0)
            {
                Debug.LogError("No music clips found!");

                return;
            }

            int randomIndex;

            do
                randomIndex = Random.Range(0, musicClips.Length);
            while (randomIndex == currentClipIndex);

            currentClipIndex = randomIndex;
            AudioClip newClip = musicClips[currentClipIndex];

            if (newClip != null)
                StartCoroutine(FadeOutAndChangeClip(newClip));
            else
                Debug.LogWarning("AudioClip not found!");
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

            Invoke("PlayRandomMusic", newClip.length);
        }
    }
}