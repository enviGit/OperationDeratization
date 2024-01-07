using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    public class ChangeColliderMesh : MonoBehaviour
    {
        [ContextMenu("Change Mesh Colliders based on Mesh Filter")]
        private void GenerateMeshColliders()
        {
            MeshFilter[] meshFilters = GameObject.FindObjectsOfType<MeshFilter>();

            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh != null && meshFilter.sharedMesh.name.StartsWith("Optimized"))
                {
                    MeshCollider meshCollider = meshFilter.GetComponent<MeshCollider>();

                    if (meshCollider == null)
                    {
                        Debug.LogWarning($"GameObject '{meshFilter.gameObject.name}' does not have a MeshCollider. Skipping.");
                        continue;
                    }

                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                    Debug.Log($"Mesh Collider assigned for GameObject: {meshFilter.gameObject.name}");
                }
            }

            Debug.Log("Mesh Colliders generated.");
        }
    }
}
