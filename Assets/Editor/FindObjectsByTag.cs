using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class FindObjectsByTag : EditorWindow
{
    private string targetTag = "Untagged";
    private int selectedLayerIndex = 0;
    private string[] layerNames;

    [MenuItem("Tools/Envi/Find Objects by Tag")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectsByTag>("Find Objects by Tag");
    }
    private void OnEnable()
    {
        layerNames = GetLayerNames();
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Find Objects with Tag", EditorStyles.boldLabel);
        targetTag = EditorGUILayout.TagField("Target Tag:", targetTag);
        EditorGUILayout.Space();
        GUILayout.Label("Sort Assets by Layer", EditorStyles.boldLabel);
        selectedLayerIndex = EditorGUILayout.Popup("Select Layer:", selectedLayerIndex, layerNames);
        EditorGUILayout.Space();

        if (GUILayout.Button("Find Objects"))
        {
            ClearConsole();
            FindObjectsWithTagAndSortByLayer();
        }
    }
    private void FindObjectsWithTagAndSortByLayer()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(targetTag);
        List<GameObject> filteredObjects = new List<GameObject>();

        if (selectedLayerIndex > 0)
        {
            int selectedLayer = selectedLayerIndex - 1;
            string selectedLayerName = layerNames[selectedLayer];
            filteredObjects = objectsWithTag
                .Where(obj => obj.layer == selectedLayer)
                .OrderBy(obj => obj.name)
                .ToList();
        }
        else
            filteredObjects = objectsWithTag.OrderBy(obj => obj.name).ToList();

        Selection.objects = filteredObjects.ToArray();

        foreach (var obj in filteredObjects)
            Debug.Log($"Object {obj.name} with tag {targetTag} and on layer {LayerMask.LayerToName(obj.layer)}");
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