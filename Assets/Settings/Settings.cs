using UnityEngine;

public class Settings
{
    private static string mouseSens = "mouseSens";
    private static string sfxMixer = "sfxMixer";
    private static string musicMixer = "musicMixer";
    private static string brightness = "brightness";

    public static float Sensitivity
    {
        get
        {
            return PlayerPrefs.GetFloat(mouseSens, 5);
        }
        set
        {
            PlayerPrefs.SetFloat(mouseSens, value);
        }
    }
    public static float SfxMixer
    {
        get
        {
            return PlayerPrefs.GetFloat(sfxMixer, 100);
        }
        set
        {
            PlayerPrefs.SetFloat(sfxMixer, value);
        }
    }
    public static float MusicMixer
    {
        get
        {
            return PlayerPrefs.GetFloat(musicMixer, 100);
        }
        set
        {
            PlayerPrefs.SetFloat(musicMixer, value);
        }
    }
    public static float Brightness
    {
        get
        {
            return PlayerPrefs.GetFloat(brightness, 0);
        }
        set
        {
            PlayerPrefs.SetFloat(brightness, value);
        }
    }
    //Function with bool type example
    /*
    private static string turboMode = "turboMode";
    public static bool EnableTurboMode
    {
        get
        {
            return PlayerPrefs.GetInt(turboMode, 1) != 0;
        }
        set
        {
            PlayerPrefs.SetInt(turboMode, value ? 1 : 0);
        }
    }*/
}