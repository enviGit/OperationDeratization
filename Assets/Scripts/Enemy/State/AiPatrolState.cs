using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiPatrolState : AiState
    {
        private float wanderRadius = 10f;

        public AiStateId GetId()
        {
            return AiStateId.Patrol;
        }
        public void Enter(AiAgent agent)
        {
            agent.weapons.ActiveWeapon();
            agent.navMeshAgent.speed = agent.config.patrolSpeed;
            agent.navMeshAgent.ResetPath();
        }
        public void Update(AiAgent agent)
        {
            if (!agent.navMeshAgent.hasPath)
            {
                Vector3 randomPoint = agent.RandomNavmeshLocation(wanderRadius);
                agent.navMeshAgent.SetDestination(randomPoint);
            }
            if (agent.targeting.HasTarget)
                agent.stateMachine.ChangeState(AiStateId.AttackTarget);
            if (agent.weapons.IsLowAmmo())
                agent.stateMachine.ChangeState(AiStateId.FindAmmo);
            if (agent.health.IsLowHealth())
                agent.stateMachine.ChangeState(AiStateId.FindFirstAidKit);
        }
        public void Exit(AiAgent agent)
        {

        }
    }
}