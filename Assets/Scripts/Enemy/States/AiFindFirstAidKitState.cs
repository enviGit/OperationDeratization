using UnityEngine;
using UnityEngine.AI;

public class AiFindFirstAidKitState : AiState
{
    private GameObject pickup;
    private GameObject[] pickups = new GameObject[3];
    private float wanderRadius = 100f;

    public AiStateId GetId()
    {
        return AiStateId.FindFirstAidKit;
    }
    public void Enter(AiAgent agent)
    {
        pickup = null;
        agent.navMeshAgent.speed = agent.config.findWeaponSpeed;
    }
    public void Exit(AiAgent agent)
    {

    }
    public void Update(AiAgent agent)
    {
        //Find pickup
        if (!pickup)
        {
            pickup = FindPickup(agent);

            if (pickup)
                CollectPickup(agent, pickup);
        }
        //Wander
        if (!agent.navMeshAgent.hasPath)
        {
            Vector3 randomPoint = RandomNavmeshLocation(wanderRadius, agent);
            agent.navMeshAgent.SetDestination(randomPoint);
        }
        if (!agent.health.IsLowHealth())
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
    }
    private Vector3 RandomNavmeshLocation(float radius, AiAgent agent)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += agent.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(agent.transform.position, randomDirection - agent.transform.position, out hit, radius))
        {
            if (hit.collider.CompareTag("GasParticles"))
                return RandomNavmeshLocation(radius, agent);
        }

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas);

        return navHit.position;
    }
    private GameObject FindPickup(AiAgent agent)
    {
        int count = agent.sightSensor.Filter(pickups, "Interactable", "FirstAidKit");

        if (count > 0)
            return pickups[0];

        return null;
    }
    private void CollectPickup(AiAgent agent, GameObject pickup)
    {
        agent.navMeshAgent.destination = pickup.transform.position;
    }
}