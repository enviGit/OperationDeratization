using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    public class Settings
    {
        private static string mouseSens = "mouseSens";
        private static string sfxMixer = "sfxMixer";
        private static string musicMixer = "musicMixer";
        private static string brightness = "brightness";
        private static string fullScreen = "fullScreen";
        private static string resolutionIndex = "resolutionIndex";
        private static string runInBg = "runInBg";
        private static string quality = "quality";
        private static string canSkipCutscene = "canSkipCutscene";
        private static string vsyncCount = "vsyncCount";

        public static float Sensitivity
        {
            get => PlayerPrefs.GetFloat(mouseSens, 1);
            set => PlayerPrefs.SetFloat(mouseSens, value);
        }
        public static float SfxMixer
        {
            get => PlayerPrefs.GetFloat(sfxMixer, 100);
            set => PlayerPrefs.SetFloat(sfxMixer, value);
        }
        public static float MusicMixer
        {
            get => PlayerPrefs.GetFloat(musicMixer, 100);
            set => PlayerPrefs.SetFloat(musicMixer, value);
        }
        public static float Brightness
        {
            get => PlayerPrefs.GetFloat(brightness, 0);
            set => PlayerPrefs.SetFloat(brightness, value);
        }
        public static bool FullScreen
        {
            get => PlayerPrefs.GetInt(fullScreen, 1) != 0;
            set => PlayerPrefs.SetInt(fullScreen, value ? 1 : 0);
        }
        public static int ResolutionIndex
        {
            get
            {
                int defaultResolutionIndex = GetDefaultResolutionIndex();

                return PlayerPrefs.GetInt(resolutionIndex, defaultResolutionIndex);
            }
            set => PlayerPrefs.SetInt(resolutionIndex, value);
        }
        private static int GetDefaultResolutionIndex()
        {
            Resolution[] resolutions = Screen.resolutions;
            int[] commonWidths = { 3840, 2560, 1920, 1366 };
            int[] commonHeights = { 2160, 1440, 1080, 768 };

            for (int i = 0; i < commonWidths.Length; i++)
            {
                for (int j = 0; j < resolutions.Length; j++)
                {
                    if (resolutions[j].width == commonWidths[i] && resolutions[j].height == commonHeights[i])
                        return j;
                }
            }

            return resolutions.Length > 0 ? resolutions.Length - 1 : 0;
        }
        public static bool RunInBg
        {
            get => PlayerPrefs.GetInt(runInBg, 1) != 0;
            set => PlayerPrefs.SetInt(runInBg, value ? 1 : 0);
        }
        public static int QualityPreset
        {
            get => PlayerPrefs.GetInt(quality, 1);
            set => PlayerPrefs.SetInt(quality, value);
        }
        public static bool CanSkipCutscene
        {
            get => PlayerPrefs.GetInt(canSkipCutscene, 0) != 0;
            set => PlayerPrefs.SetInt(canSkipCutscene, value ? 1 : 0);
        }
        public static bool VSync
        {
            get => PlayerPrefs.GetInt(vsyncCount, 0) != 0;
            set => PlayerPrefs.SetInt(vsyncCount, value ? 1 : 0);
        }
    }
}