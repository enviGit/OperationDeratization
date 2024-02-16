using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    [ExecuteInEditMode]
    public class AiSightSensor : MonoBehaviour
    {
        public float distance = 75f;
        public float angle = 60f;
        public float height = 3.5f;
        public Color meshColor = Color.red;
        public int scanFrequency = 30;
        public LayerMask layers;
        public LayerMask occlusionLayers;
        public List<GameObject> Objects
        {
            get
            {
                objects.RemoveAll(obj => !obj || !IsValidTarget(obj));

                return objects;
            }
        }
        private List<GameObject> objects = new List<GameObject>();
        private Collider[] colliders = new Collider[50];
        private Mesh mesh;
        private int count;
        private float scanInterval;
        private float scanTimer;

        private void Start()
        {
            scanInterval = 1f / scanFrequency;
        }
        private void Update()
        {
            scanTimer -= Time.deltaTime;

            if (scanTimer < 0)
            {
                scanTimer += scanInterval;
                Scan();
            }
        }
        private void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(transform.position + Vector3.down * 0.5f, distance, colliders, layers, QueryTriggerInteraction.Collide);
            objects.Clear();

            for (int i = 0; i < count; ++i)
            {
                GameObject obj = colliders[i].gameObject;

                if (IsInSight(obj) && IsValidTarget(obj))
                    objects.Add(obj);
            }
        }
        private bool IsValidTarget(GameObject obj)
        {
            return obj != gameObject && (obj.CompareTag("Weapon") || obj.CompareTag("AmmoBox") || obj.CompareTag("FirstAidKit") || obj.CompareTag("Armor") || obj.CompareTag("Enemy") || obj.CompareTag("Player"));
        }
        public bool IsInSight(GameObject obj)
        {
            Vector3 origin = transform.position + Vector3.down * 0.5f;
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;

            if (direction.y < 0 || direction.y > height)
                return false;

            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, transform.forward);

            if (deltaAngle > angle)
                return false;

            origin.y += height / 2;
            dest.y = origin.y;

            if (Physics.Linecast(origin, dest, occlusionLayers))
                return false;

            return true;
        }
        private Mesh CreateWedgeMesh()
        {
            Mesh mesh = new Mesh();
            int segments = 10;
            int numTriangles = (segments * 4) + 2 + 2;
            int numVertices = numTriangles * 3;
            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];
            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
            Vector3 topCenter = bottomCenter + Vector3.up * height + Vector3.down * 0.5f;
            Vector3 topRight = bottomRight + Vector3.up * height + Vector3.down * 0.5f;
            Vector3 topLeft = bottomLeft + Vector3.up * height + Vector3.down * 0.5f;
            int vert = 0;
            //LeftSide
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;
            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;
            //RightSide
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;
            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;
            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;

            for (int i = 0; i < segments; ++i)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
                topLeft = bottomLeft + Vector3.up * height + Vector3.down * 0.5f;
                topRight = bottomRight + Vector3.up * height + Vector3.down * 0.5f;
                //FarSide
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;
                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;
                //Top
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;
                //Bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;
                currentAngle += deltaAngle;
            }
            for (int i = 0; i < numVertices; ++i)
                triangles[i] = i;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
        private void OnValidate()
        {
            mesh = CreateWedgeMesh();
            scanInterval = 1f / scanFrequency;
        }
        private void OnDrawGizmos()
        {
            if (mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, transform.position + Vector3.down * 0.5f, transform.rotation);
            }

            Gizmos.color = Color.green;

            foreach (var obj in Objects)
                Gizmos.DrawSphere(obj.transform.position, 0.3f);
        }
        public int Filter(GameObject[] buffer, string layerName, string tagName = null)
        {
            int layer = LayerMask.NameToLayer(layerName);
            int count = 0;

            foreach (var obj in Objects)
            {
                if (tagName != null && !obj.CompareTag(tagName))
                    continue;
                if (obj.layer == layer)
                    buffer[count++] = obj;
                if (buffer.Length == count)
                    break;
            }

            return count;
        }
    }
}