/*using UnityEngine;

public class AiFindWeaponState : AiState
{
    private Weapon currentWeapon;
    private Vector3 currentDestination;

    public AiStateId GetId()
    {
        return AiStateId.FindWeapon;
    }
    public void Enter(AiAgent agent)
    {
        currentWeapon = FindClosestWeapon(agent);

        if (currentWeapon != null)
        {
            currentDestination = currentWeapon.transform.position;
            agent.navMeshAgent.SetDestination(currentDestination);
            agent.navMeshAgent.speed = 4f;
        }
    }
    public void Exit(AiAgent agent)
    {
        currentWeapon = null;
    }
    public void Update(AiAgent agent)
    {
        if (agent.weapons.HasWeapon())
        {
            agent.stateMachine.ChangeState(AiStateId.AttackPlayer);

            return;
        }
        if (currentWeapon != null && Vector3.Distance(agent.transform.position, currentDestination) <= agent.navMeshAgent.stoppingDistance)
        {
            agent.navMeshAgent.isStopped = true;
            currentWeapon = null;
        }
        if (currentWeapon == null)
        {
            currentWeapon = FindClosestWeapon(agent);

            if (currentWeapon != null)
            {
                currentDestination = currentWeapon.transform.position;
                agent.navMeshAgent.SetDestination(currentDestination);
                agent.navMeshAgent.isStopped = false;
            }
        }
    }
    private Weapon FindClosestWeapon(AiAgent agent)
    {
        Weapon[] weapons = Object.FindObjectsOfType<Weapon>();
        Weapon closestWeapon = null;
        float closestDistance = float.MaxValue;

        foreach (var weapon in weapons)
        {
            if (weapon.gun.gunStyle == GunStyle.Grenade || weapon.gun.gunStyle == GunStyle.Flashbang || weapon.gun.gunStyle == GunStyle.Smoke)
                continue;

            float distanceToWeapon = Vector3.Distance(agent.transform.position, weapon.transform.position);

            if (distanceToWeapon < closestDistance)
            {
                closestDistance = distanceToWeapon;
                closestWeapon = weapon;
            }
        }

        return closestWeapon;
    }
}*/

using UnityEngine;
using UnityEngine.AI;

public class AiFindWeaponState : AiState
{
    private Weapon currentWeapon;
    private Vector3 currentDestination;
    private GameObject pickup;
    private GameObject[] pickups = new GameObject[1];
    private float wanderRadius = 100f;

    public AiStateId GetId()
    {
        return AiStateId.FindWeapon;
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
        if (!pickup)
        {
            pickup = FindPickup(agent);

            if (pickup)
                CollectPickup(agent, pickup);
        }
        if (!agent.navMeshAgent.hasPath)
        {
            Vector3 randomPoint = RandomNavmeshLocation(wanderRadius, agent);
            agent.navMeshAgent.SetDestination(randomPoint);
        }
        if (agent.weapons.HasWeapon())
            agent.stateMachine.ChangeState(AiStateId.AttackPlayer);
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
        int count = agent.sightSensor.Filter(pickups, "Interactable");

        if (count > 0)
            return pickups[0];

        return null;
    }
    private void CollectPickup(AiAgent agent, GameObject pickup)
    {
        agent.navMeshAgent.destination = pickup.transform.position;
    }
}