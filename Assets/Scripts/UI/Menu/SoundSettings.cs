using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class SoundSettings : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private AudioMixer musicMixer;
        private TextMeshProUGUI musicText;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private AudioMixer sfxMixer;
        private TextMeshProUGUI sfxText;
        private float originalMusicVolume;
        private float originalSfxVolume;

        private void Start()
        {
            musicSource.Play();
            musicText = musicSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            sfxText = sfxSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            originalMusicVolume = Settings.MusicMixer;
            originalSfxVolume = Settings.SfxMixer;
            SetMusicVolume(originalMusicVolume);
            musicText.text = musicSlider.value.ToString("0");
            musicSlider.onValueChanged.AddListener((v) => { musicText.text = v.ToString("0"); });
            SetSfxVolume(originalSfxVolume);
            sfxText.text = sfxSlider.value.ToString("0");
            sfxSlider.onValueChanged.AddListener((v) => { sfxText.text = v.ToString("0"); });
        }
        public void SetMusicVolume(float _value)
        {
            if (_value < 1)
                _value = .001f;

            RefreshSlider(_value, musicSlider);
            musicMixer.SetFloat("MusicVolume", Mathf.Log10(_value / 100) * 20f);
        }
        public void SetSfxVolume(float _value)
        {
            if (_value < 1)
                _value = .001f;

            RefreshSlider(_value, sfxSlider);
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
        public void RestoreOriginalValues()
        {
            SetMusicVolume(originalMusicVolume);
            SetSfxVolume(originalSfxVolume);
            Settings.MusicMixer = originalMusicVolume;
            Settings.SfxMixer = originalSfxVolume;
        }
        public void ApplyChanges()
        {
            originalMusicVolume = musicSlider.value;
            originalSfxVolume = sfxSlider.value;
            Settings.MusicMixer = originalMusicVolume;
            Settings.SfxMixer = originalSfxVolume;
        }
    }
}