using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class FindObjectsByLayer : EditorWindow
{
    private int selectedLayerIndex = 0;
    private string[] layerNames;
    private string targetTag = "Untagged";
    private bool useTag = false;

    [MenuItem("Tools/Envi/Find Objects by Layer")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectsByLayer>("Find Objects by Layer");
    }
    private void OnEnable()
    {
        layerNames = GetLayerNames();
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Find Objects with Layer", EditorStyles.boldLabel);
        selectedLayerIndex = EditorGUILayout.Popup("Target Layer:", selectedLayerIndex, layerNames);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        useTag = EditorGUILayout.Toggle("Use Tag", useTag);

        if (useTag)
        {
            GUILayout.Label("Sort Assets by Tag", EditorStyles.boldLabel);
            targetTag = EditorGUILayout.TagField("Select Tag", targetTag);
            EditorGUILayout.Space();
        }
        if (GUILayout.Button("Find Objects"))
        {
            ClearConsole();
            FindObjectsByLayerAndCriteria();
        }
    }
    private void FindObjectsByLayerAndCriteria()
    {
        GameObject[] objectsInLayer = GameObject.FindObjectsOfType<GameObject>()
            .Where(obj => obj.layer == selectedLayerIndex - 1)
            .OrderBy(obj => obj.name)
            .ToArray();
        List<GameObject> filteredObjects = new List<GameObject>();

        foreach (var obj in objectsInLayer)
        {
            if(useTag)
                if (!string.IsNullOrEmpty(targetTag) && obj.CompareTag(targetTag) == false)
                    continue;

            filteredObjects.Add(obj);
        }

        Selection.objects = filteredObjects.ToArray();

        foreach (var obj in filteredObjects)
            Debug.Log($"Object {obj.name} on layer {LayerMask.LayerToName(obj.layer)} with tag {obj.tag}");
    }
    private string[] GetLayerNames()
    {
        string[] layerNames = new string[33];
        layerNames[0] = "None";

        for (int i = 0; i < 32; i++)
            layerNames[i + 1] = LayerMask.LayerToName(i);

        return layerNames;
    }
    private void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
}