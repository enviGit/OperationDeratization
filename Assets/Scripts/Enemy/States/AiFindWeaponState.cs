using UnityEngine;
using UnityEngine.AI;

public class AiFindWeaponState : AiState
{
    private GameObject pickup;
    private GameObject[] pickups = new GameObject[3];
    private float wanderRadius = 10f;
    private float stopDistance = 5f;
    private float distanceCounter = 0f;

    public AiStateId GetId()
    {
        return AiStateId.FindWeapon;
    }
    public void Enter(AiAgent agent)
    {
        pickup = null;
        agent.navMeshAgent.speed = agent.config.findWeaponSpeed;
        distanceCounter = 0f;
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

        distanceCounter += agent.navMeshAgent.velocity.magnitude * Time.deltaTime;

        if (distanceCounter >= stopDistance)
        {
            agent.navMeshAgent.ResetPath();
            distanceCounter = 0f;
        }
        if (agent.weapons.HasWeapon())
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
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
    private GameObject FindPickup(AiAgent agent)
    {
        int count = agent.sightSensor.Filter(pickups, "Interactable", "Weapon");

        if (count > 0)
            return pickups[0];

        return null;
    }
    private void CollectPickup(AiAgent agent, GameObject pickup)
    {
        agent.navMeshAgent.destination = pickup.transform.position;
    }
}