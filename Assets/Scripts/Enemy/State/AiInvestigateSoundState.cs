using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiInvestigateSoundState : AiState
    {
        private float timeSinceLastHeardTarget = 0f;
        private float maxTimeWithoutTarget = 5f;

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
            if (!agent.audioSensor.LastDetectedSoundAudible)
                agent.stateMachine.ChangeState(AiStateId.Patrol);
            if (agent.targeting.HasTarget)
            {
                agent.stateMachine.ChangeState(AiStateId.AttackTarget);
                timeSinceLastHeardTarget = 0f; // Reset timer
            }
            else
            {
                if (Vector3.Distance(agent.audioSensor.LastDetectedSoundPosition, agent.transform.position) < 2.5f)
                {
                    timeSinceLastHeardTarget += Time.deltaTime;

                    if (timeSinceLastHeardTarget >= maxTimeWithoutTarget)
                    {
                        if (agent.health.IsLowHealth())
                        {
                            agent.stateMachine.ChangeState(AiStateId.FindFirstAidKit);
                            timeSinceLastHeardTarget = 0f;
                        }
                        else
                        {
                            agent.stateMachine.ChangeState(AiStateId.Patrol);
                            timeSinceLastHeardTarget = 0f;
                        }
                    }
                }
            }

            agent.CheckAndPlayRandomClip();
        }
        public void Exit(AiAgent agent)
        {

        }
    }
}