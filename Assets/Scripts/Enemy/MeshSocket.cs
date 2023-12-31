using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class MeshSocket : MonoBehaviour
    {
        public MeshSockets.SocketId socketId;
        public HumanBodyBones bone;
        public Vector3 offset;
        public Vector3 rotation;
        Transform attachPoint;

        private void Start()
        {
            Animator animator = GetComponentInParent<Animator>();
            attachPoint = new GameObject("socket" + socketId).transform;
            attachPoint.SetParent(animator.GetBoneTransform(bone));
            attachPoint.localPosition = offset;
            attachPoint.localRotation = Quaternion.Euler(rotation);
        }
        public void Attach(Transform objectTransform)
        {
            objectTransform.SetParent(attachPoint, false);
        }
    }
}