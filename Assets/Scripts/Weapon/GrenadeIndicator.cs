using UnityEngine;

public class GrenadeIndicator : MonoBehaviour
{
    [Header("Indicator")]
    private GameObject indicator;

    private void Start()
    {
        indicator = new GameObject("GrenadeIndicator");
        indicator.transform.parent = transform;
        indicator.transform.localPosition = Vector3.zero;
        MeshRenderer renderer = indicator.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Unlit/Color"));
        renderer.material.color = Color.white;
        float size = 0.5f;
        MeshFilter filter = indicator.AddComponent<MeshFilter>();
        filter.mesh = CreateCircleMesh(size);
    }
    /*private void Update()
    {
        Vector3 indicatorPos = transform.position + Vector3.up * 0.3f;
        indicator.transform.position = indicatorPos;
        Vector3 playerPos = Camera.main.transform.position;
        playerPos.y = indicator.transform.position.y;
        Vector3 lookPos = playerPos - indicatorPos;
        Quaternion rotation = Quaternion.LookRotation(lookPos, Vector3.up);
        rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        indicator.transform.rotation = rotation;
    }*/
    private Mesh CreateCircleMesh(float size)
    {
        int segments = 32;
        float angle = 2f * Mathf.PI / segments;
        float radius = size / 2f;
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        vertices[0] = Vector3.zero;
        for (int i = 1; i <= segments; i++)
        {
            float x = Mathf.Sin(angle * i) * radius;
            float z = Mathf.Cos(angle * i) * radius;
            vertices[i] = new Vector3(x, 0f, z);
            triangles[(i - 1) * 3] = 0;
            triangles[(i - 1) * 3 + 1] = i;
            triangles[(i - 1) * 3 + 2] = i == segments ? 1 : i + 1;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }
}