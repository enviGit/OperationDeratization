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
        Screen.fullScreen = Settings.FullScreen;
        musicVolume = Settings.MusicMixer;

        if (musicVolume < 1)
            musicVolume = .001f;

        musicMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume / 100) * 20f);
        bgMusic = GetComponent<AudioSource>();
        bgMusic.Play();
        sfxVolume = Settings.SfxMixer;

        if (sfxVolume < 1)
            sfxVolume = .001f;

        sfxMixer.SetFloat("SfxVolume", Mathf.Log10(sfxVolume / 100) * 20f);

        if (postProcessing.TryGet(out colorAdj))
            colorAdj.postExposure.value = Settings.Brightness;
    }
    private void ApplyResolution(int resolution)
    {
        Screen.SetResolution(resolutions[resolution].width, resolutions[resolution].height, Settings.FullScreen);
    }
}
