using UnityEngine;

public class AiDeathState : AiState
{
    public Vector3 direction;

    public AiStateId GetId()
    {
        return AiStateId.Death;
    }
    public void Enter(AiAgent agent)
    {
        if (agent.navMeshAgent.enabled)
        {
            agent.navMeshAgent.isStopped = true;
            agent.ragdoll.ActivateRagdoll();
            direction.y = 1;
            agent.ragdoll.ApplyForce(direction * agent.config.dieForce);
            agent.weapons.DropWeapon();
        }
    }
    public void Exit(AiAgent agent)
    {

    }
    public void Update(AiAgent agent)
    {

    }
}
