using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Optimization.FloatingOrigin
{
    public class AdjustObjectsPosition : MonoBehaviour
    {
        private Vector3 playerStartPosition;
        private GameObject targetObject;
        private float threshold = 65f;

        private void Start()
        {
            targetObject = GameObject.Find("3D");

            if (targetObject == null)
                Debug.LogError("Object with name not found: 3D");

            SetPlayerPosition();
        }
        private void LateUpdate()
        {
            SetPlayerPosition();
        }
        private void SetPlayerPosition()
        {
            float distanceToZero = Vector3.Distance(transform.position, Vector3.zero);

            if (distanceToZero > threshold || distanceToZero < -threshold)
            {
                playerStartPosition = transform.position;
                transform.position = Vector3.zero;
                AdjustObjectPosition();
            }
        }
        private void AdjustObjectPosition()
        {
            if (targetObject != null)
            {
                Vector3 positionDifference = targetObject.transform.position - playerStartPosition;
                targetObject.transform.position = transform.position + positionDifference;

                //Also we would need to bake NavMeshSurface realtime

                foreach (Transform child in targetObject.transform.GetChild(0))
                {
                    NavMeshAgent navMeshAgent = child.GetComponent<NavMeshAgent>();

                    if (navMeshAgent != null)
                    {
                        navMeshAgent.Warp(child.position);

                        if (navMeshAgent.hasPath)
                            navMeshAgent.SetDestination(navMeshAgent.destination);
                    }
                }
            }
        }
    }
}