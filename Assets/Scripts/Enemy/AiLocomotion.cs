using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (agent.hasPath)
            animator.SetFloat("Speed", agent.velocity.magnitude);
        else
            animator.SetFloat("Speed", 0);
    }
}
