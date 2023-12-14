using UnityEngine;
using System.Collections;


public class Fps : MonoBehaviour
{
    private float count;
    private float gpuUsage;
    private long totalMemory;
    private long freeMemory;

    private IEnumerator Start()
    {
        GUI.depth = 2;

        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            MonitorMemoryUsage();

            yield return new WaitForSeconds(1f);
        }
    }
    private void OnGUI()
    {
        GUIStyle headStyle = new GUIStyle();
        headStyle.fontSize = 30;
        headStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(5, 5, 100, 25), "FPS: " + Mathf.Round(count), headStyle);
        GUI.Label(new Rect(5, 35, 100, 25), $"Total Mem: {totalMemory} MB", headStyle);
        GUI.Label(new Rect(5, 65, 100, 25), $"Free Mem: {freeMemory} MB", headStyle);
    }
    private void MonitorMemoryUsage()
    {
        totalMemory = SystemInfo.systemMemorySize;
        freeMemory = System.GC.GetTotalMemory(false);
    }
}