public class AiAttackPlayerState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.AttackPlayer;
    }
    public void Enter(AiAgent agent)
    {
        agent.weapons.ActiveWeapon();
        agent.weapons.SetTarget(agent.playerTransform);
        agent.navMeshAgent.stoppingDistance = 5f;
        agent.weapons.SetFiring(true);
    }
    public void Update(AiAgent agent)
    {
        agent.navMeshAgent.destination = agent.playerTransform.position;

        if (agent.playerTransform.GetComponent<PlayerHealth>().isAlive == false)
            agent.stateMachine.ChangeState(AiStateId.Idle);
    }
    public void Exit(AiAgent agent)
    {
        //agent.navMeshAgent.stoppingDistance = 0;
    }
}
