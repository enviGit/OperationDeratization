using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    public class LookAtPlayer : MonoBehaviour
    {
        private Transform player;
        public float rotationSpeed = 1f;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        private void LateUpdate()
        {
            SmoothLookAtPlayer();
        }
        private void SmoothLookAtPlayer()
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        private void LookAt()
        {
            transform.LookAt(player);
        }
    }
}