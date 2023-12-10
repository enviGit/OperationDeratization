using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerSettings : MonoBehaviour
{
    private Resolution[] resolutions;
    private int resolutionIndex;
    public AudioMixer musicMixer;
    private float musicVolume;
    private AudioSource bgMusic;
    public AudioMixer sfxMixer;
    private float sfxVolume;
    public VolumeProfile postProcessing;
    private ColorAdjustments colorAdj;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionIndex = Settings.ResolutionIndex;
        ApplyResolution(resolutionIndex);
        ApplyFullscreen();
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
    private void ApplyFullscreen()
    {
        Screen.fullScreen = Settings.FullScreen;
    }
    private void ApplyMusicVolume(float volume)
    {
        if (volume < 1)
            volume = .001f;

        musicMixer.SetFloat("MusicVolume", Mathf.Log10(volume / 100) * 20f);
        bgMusic = GetComponent<AudioSource>();
        bgMusic.Play();
    }
    private void ApplySfxVolume(float volume)
    {
        if (sfxVolume < 1)
            sfxVolume = .001f;

        sfxMixer.SetFloat("SfxVolume", Mathf.Log10(sfxVolume / 100) * 20f);
    }
    private void ApplyBrightness()
    {
        if (postProcessing.TryGet(out colorAdj))
            colorAdj.postExposure.value = Settings.Brightness;
    }
}
