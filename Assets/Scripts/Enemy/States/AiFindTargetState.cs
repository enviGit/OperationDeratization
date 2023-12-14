using UnityEngine;
using UnityEngine.AI;

public class AiFindTargetState : AiState
{
    private float wanderRadius = 10f;
    private float stopDistance = 5f;
    private float distanceCounter = 0f;

    public AiStateId GetId()
    {
        return AiStateId.FindTarget;
    }
    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.findTargetSpeed;
        distanceCounter = 0f;
    }
    public void Update(AiAgent agent)
    {
        //Wander
        if (!agent.navMeshAgent.hasPath)
        {
            Vector3 randomPoint = RandomNavmeshLocation(wanderRadius, agent);
            agent.navMeshAgent.SetDestination(randomPoint);
        }
        distanceCounter += agent.navMeshAgent.velocity.magnitude * Time.deltaTime;

        if (distanceCounter >= stopDistance)
        {
            agent.navMeshAgent.ResetPath();
            distanceCounter = 0f;
        }
        if (agent.targeting.HasTarget)
            agent.stateMachine.ChangeState(AiStateId.AttackTarget);
    }
    public void Exit(AiAgent agent)
    {

    }
    private Vector3 RandomNavmeshLocation(float radius, AiAgent agent)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += agent.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(agent.transform.position, randomDirection - agent.transform.position, out hit, radius * 20f))
        {
            if (hit.collider.CompareTag("GasParticles"))
                return RandomNavmeshLocation(radius, agent);
        }

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas);

        return navHit.position;
    }
}
