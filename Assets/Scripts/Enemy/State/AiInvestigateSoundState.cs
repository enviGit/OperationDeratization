namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiInvestigateSoundState : AiState
    {
        public AiStateId GetId()
        {
            return AiStateId.InvestigateSound;
        }
        public void Enter(AiAgent agent)
        {
            agent.weapons.ActiveWeapon();
            agent.navMeshAgent.speed = agent.config.patrolSpeed;
            agent.navMeshAgent.stoppingDistance = 2f;
            agent.navMeshAgent.SetDestination(agent.audioSensor.LastDetectedSoundPosition);
        }
        public void Update(AiAgent agent)
        {
            if (!agent.navMeshAgent.hasPath || !agent.audioSensor.LastDetectedSoundAudible)
                agent.stateMachine.RevertToPreviousState();
            if (agent.targeting.HasTarget)
                agent.stateMachine.ChangeState(AiStateId.AttackTarget);
        }
        public void Exit(AiAgent agent)
        {

        }
    }
}