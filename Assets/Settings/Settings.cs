using UnityEngine;

public class Settings
{
    private static string mouseSens = "mouseSens";
    private static string sfxMixer = "sfxMixer";
    private static string musicMixer = "musicMixer";
    private static string brightness = "brightness";
    private static string fullScreen = "fullScreen";
    private static string resolutionIndex = "resolutionIndex";
    
    public static float Sensitivity
    {
        get => PlayerPrefs.GetFloat(mouseSens, 5);
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
        get => PlayerPrefs.GetInt(resolutionIndex, 0);
        set => PlayerPrefs.SetInt(resolutionIndex, value);
    }
}