using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class AiLocomotion : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            SetSpeed();
        }
        private void SetSpeed()
        {
            if (agent.hasPath)
                animator.SetFloat("Speed", agent.velocity.magnitude);
            else
                animator.SetFloat("Speed", 0);
        }
    }
}