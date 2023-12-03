using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour
{
    private float count;

    private IEnumerator Start()
    {
        GUI.depth = 2;

        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(1f);
        }
    }
    private void OnGUI()
    {
        GUIStyle headStyle = new GUIStyle();
        headStyle.fontSize = 30;
        headStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(5, 5, 100, 25), "FPS: " + Mathf.Round(count), headStyle);
    }
}