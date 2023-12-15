using UnityEngine;
using UnityEngine.AI;

public class AiFindTargetState : AiState
{
    private float wanderRadius = 10f;

    public AiStateId GetId()
    {
        return AiStateId.FindTarget;
    }
    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.findTargetSpeed;
    }
    public void Update(AiAgent agent)
    {
        //Wander
        if (!agent.navMeshAgent.hasPath)
        {
            Vector3 randomPoint = RandomNavmeshLocation(wanderRadius, agent);
            agent.navMeshAgent.SetDestination(randomPoint);
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
