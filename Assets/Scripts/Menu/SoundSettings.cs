using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] Slider sfxSlider;
    [SerializeField] AudioMixer sfxMixer;

    private void Start()
    {
        SetMusicVolume(Settings.MusicMixer);
        SetSfxVolume(Settings.SfxMixer);
    }
    public void SetMusicVolume(float _value)
    {
        if (_value < 1)
            _value = .001f;

        RefreshSlider(_value, musicSlider);
        Settings.MusicMixer = _value;
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(_value / 100) * 20f);
    }
    public void SetSfxVolume(float _value)
    {
        if (_value < 1)
            _value = .001f;

        RefreshSlider(_value, sfxSlider);
        Settings.SfxMixer = _value;
        sfxMixer.SetFloat("SfxVolume", Mathf.Log10(_value / 100) * 20f);
    }
    public void SetVolumeFromMusicSlider()
    {
        SetMusicVolume(musicSlider.value);
    }
    public void SetVolumeFromSfxSlider()
    {
        SetSfxVolume(sfxSlider.value);
    }
    public void RefreshSlider(float _value, Slider slider)
    {
        slider.value = _value;
    }
}