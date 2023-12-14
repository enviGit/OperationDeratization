using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindObjectsByStaticFlag : EditorWindow
{
    private bool searchStatic = false;
    private string targetTag = "Untagged";
    private bool useTag = false;
    private bool useLayerMask = false;
    private int selectedLayerIndex = 0;
    private string[] layerNames;

    [MenuItem("Tools/Envi/Find Objects by Static Flag")]
    public static void ShowStaticObjectsWindow()
    {
        GetWindow<FindObjectsByStaticFlag>("Find Objects by Static Flag");
    }
    private void OnEnable()
    {
        layerNames = GetLayerNames();
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Find Objects with Static Flag", EditorStyles.boldLabel);

        if (GUILayout.Button("Search Static Objects"))
        {
            ClearConsole();
            searchStatic = true;
            FindObjectsByStaticAndCriteria();
        }

        if (GUILayout.Button("Search Non-Static Objects"))
        {
            ClearConsole();
            searchStatic = false;
            FindObjectsByStaticAndCriteria();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Additional Criteria", EditorStyles.boldLabel);
        useTag = EditorGUILayout.Toggle("Use Tag", useTag);

        if (useTag)
            targetTag = EditorGUILayout.TagField("Select Tag", targetTag);

        useLayerMask = EditorGUILayout.Toggle("Use Layer Mask", useLayerMask);

        if (useLayerMask)
            selectedLayerIndex = EditorGUILayout.Popup("Target Layer:", selectedLayerIndex, layerNames);

        EditorGUILayout.Space();
    }
    private void FindObjectsByStaticAndCriteria()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> filteredObjects = new List<GameObject>();

        foreach (var obj in allObjects)
        {
            if (searchStatic && !obj.isStatic)
                continue;
            if (!searchStatic && obj.isStatic)
                continue;
            if (useTag && !string.IsNullOrEmpty(targetTag) && !obj.CompareTag(targetTag))
                continue;
            if (useLayerMask && obj.layer + 1 != selectedLayerIndex && selectedLayerIndex > 0)
                continue;

            filteredObjects.Add(obj);
        }

        Selection.objects = filteredObjects.ToArray();

        foreach (var obj in filteredObjects)
            Debug.Log($"Object {obj.name} {(obj.isStatic ? "is" : "isn't")} static, with tag {obj.tag}, on layer {LayerMask.LayerToName(obj.layer)}");
    }
    private void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
    private string[] GetLayerNames()
    {
        string[] layerNames = new string[33];
        layerNames[0] = "None";

        for (int i = 0; i < 32; i++)
            layerNames[i + 1] = LayerMask.LayerToName(i);

        return layerNames;
    }
}