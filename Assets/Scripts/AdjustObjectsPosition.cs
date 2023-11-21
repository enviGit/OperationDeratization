using UnityEngine;

public class AdjustObjectsPosition : MonoBehaviour
{
    private Vector3 playerStartPosition;
    private GameObject targetObject;
    private float threshold = 65f;

    private void Start()
    {
        //playerStartPosition = transform.position;
        //transform.position = Vector3.zero;
        targetObject = GameObject.Find("3D");

        if (targetObject == null)
            Debug.LogError("Object with name not found: 3D");

        //AdjustObjectPosition();
    }
    private void Update()
    {
        if (Mathf.Abs(transform.position.x) > threshold || Mathf.Abs(transform.position.z) > threshold || Mathf.Abs(transform.position.x) < -threshold || Mathf.Abs(transform.position.z) < -threshold)
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
        }
    }
}