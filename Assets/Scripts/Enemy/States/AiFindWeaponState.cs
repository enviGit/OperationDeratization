using UnityEngine;
using UnityEngine.AI;

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
}