using UnityEngine;

public class AiFindWeaponState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.FindWeapon;
    }
    public void Enter(AiAgent agent)
    {
        Weapon pickup = FindClosestWeapon(agent);
        agent.navMeshAgent.destination = pickup.transform.position;
        agent.navMeshAgent.speed = 4f;
    }
    public void Exit(AiAgent agent)
    {
        
    }
    public void Update(AiAgent agent)
    {
        if (agent.weapons.HasWeapon())
            agent.weapons.ActiveWeapon();
    }
    private Weapon FindClosestWeapon(AiAgent agent)
    {
        Weapon[] weapons = Object.FindObjectsOfType<Weapon>();
        Weapon closestWeapon = null;
        float closestDistance = float.MaxValue;

        foreach (var weapon in weapons)
        {
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
