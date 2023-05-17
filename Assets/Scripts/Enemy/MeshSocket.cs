using UnityEngine;

public class MeshSocket : MonoBehaviour
{
    public MeshSockets.SocketId socketId;
    Transform attachPoint;

    private void Start()
    {
        attachPoint = transform.GetChild(0);
    }
    public void Attach(Transform objectTransform)
    {
        objectTransform.localScale = new Vector3(2, 2, 2);
        objectTransform.SetParent(attachPoint, false);
    }
}
