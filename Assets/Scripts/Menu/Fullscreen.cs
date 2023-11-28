using UnityEngine;

public class Fullscreen : MonoBehaviour
{
    public void Change()
    {
        Screen.fullScreen = !Screen.fullScreen;
        print("Changed screen mode");
    }
}