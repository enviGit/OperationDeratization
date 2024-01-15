using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class PlayerSettings : MonoBehaviour
    {
        private Resolution[] resolutions;
        private int resolutionIndex;
        public AudioMixer musicMixer;
        private float musicVolume;
        public AudioMixer sfxMixer;
        private float sfxVolume;
        public VolumeProfile[] postProcessing = new VolumeProfile[4];
        private ColorAdjustments[] colorAdj = new ColorAdjustments[4];

        private void Start()
        {
            resolutions = Screen.resolutions;
            resolutionIndex = Settings.ResolutionIndex;
            ApplyResolution(resolutionIndex);
            musicVolume = Settings.MusicMixer;
            ApplyMusicVolume(musicVolume);
            sfxVolume = Settings.SfxMixer;
            ApplySfxVolume(musicVolume);
            ApplyBrightness();
        }
        private void ApplyResolution(int resolution)
        {
            Screen.SetResolution(resolutions[resolution].width, resolutions[resolution].height, Settings.FullScreen);
        }
        private void ApplyMusicVolume(float volume)
        {
            if (volume < 1)
                volume = .001f;

            musicMixer.SetFloat("MusicVolume", Mathf.Log10(volume / 100) * 20f);
        }
        private void ApplySfxVolume(float volume)
        {
            if (sfxVolume < 1)
                sfxVolume = .001f;

            sfxMixer.SetFloat("SfxVolume", Mathf.Log10(sfxVolume / 100) * 20f);
        }
        private void ApplyBrightness()
        {
            for(int i = 0; i < postProcessing.Length; i++)
            {
                if (postProcessing[i].TryGet(out colorAdj[i]))
                    colorAdj[i].postExposure.value = Settings.Brightness;
            }
        }
    }
}