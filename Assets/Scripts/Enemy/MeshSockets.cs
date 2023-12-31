using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class MeshSockets : MonoBehaviour
    {
        public enum SocketId
        {
            Spine,
            RightHand
        }
        Dictionary<SocketId, MeshSocket> socketMap = new Dictionary<SocketId, MeshSocket>();

        private void Start()
        {
            MeshSocket[] sockets = GetComponentsInChildren<MeshSocket>();

            foreach (var socket in sockets)
                socketMap[socket.socketId] = socket;
        }
        public void Attach(Transform objectTransform, SocketId socketId)
        {
            socketMap[socketId].Attach(objectTransform);
        }
    }
}