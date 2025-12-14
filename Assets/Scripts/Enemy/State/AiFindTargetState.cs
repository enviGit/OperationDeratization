using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiFindTargetState : AiState
    {
        private float wanderRadius = 10f;
        private float timeSinceLastSawTarget = 0f;
        private float maxTimeWithoutTarget = 15f;

        public AiStateId GetId()
        {
            return AiStateId.FindTarget;
        }
        public void Enter(AiAgent agent)
        {
            agent.navMeshAgent.speed = agent.config.findTargetSpeed;
        }
        public void Update(AiAgent agent)
        {
            if (!agent.navMeshAgent.hasPath)
            {
                Vector3 randomPoint = agent.RandomNavmeshLocation(wanderRadius);
                agent.navMeshAgent.SetDestination(randomPoint);
            }
            if (agent.targeting.HasTarget)
            {
                timeSinceLastSawTarget = 0f;
                agent.stateMachine.ChangeState(AiStateId.AttackTarget);
            }
            else
            {
                timeSinceLastSawTarget += Time.deltaTime;

                if (timeSinceLastSawTarget >= maxTimeWithoutTarget)
                {
                    timeSinceLastSawTarget = 0f;

                    if (agent.health.IsLowHealth())
                        agent.stateMachine.ChangeState(AiStateId.FindFirstAidKit);
                    else
                        agent.stateMachine.ChangeState(AiStateId.Patrol);
                }
            }

            agent.CheckAndPlayRandomClip();
        }
        public void Exit(AiAgent agent)
        {

        }
    }
}