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
        agent.navMeshAgent.stoppingDistance = agent.config.attackStoppingDistance;
        agent.navMeshAgent.speed = agent.config.attackSpeed;
    }
    public void Update(AiAgent agent)
    {
        agent.navMeshAgent.destination = agent.playerTransform.position;
        UpdateFiring(agent);

        if (agent.playerTransform.GetComponent<PlayerHealth>().isAlive == false)
            agent.stateMachine.ChangeState(AiStateId.Idle);
    }
    private void UpdateFiring(AiAgent agent)
    {
        if(agent.sightSensor.IsInSight(agent.playerTransform.gameObject))
            agent.weapons.SetFiring(true);
        else
            agent.weapons.SetFiring(false);
    }
    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 0;
    }
}
