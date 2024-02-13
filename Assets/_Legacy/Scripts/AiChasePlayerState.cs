using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    /*public class AiChasePlayerState : AiState
    {
        float timer = 0;

        public AiStateId GetId()
        {
            return AiStateId.ChasePlayer;
        }
        public void Enter(AiAgent agent)
        {

        }
        public void Exit(AiAgent agent)
        {

        }
        public void Update(AiAgent agent)
        {
            if (!agent.enabled)
                return;

            timer -= Time.deltaTime;

            if (!agent.navMeshAgent.hasPath)
                agent.navMeshAgent.destination = agent.playerTransform.position;
            if (timer < 0)
            {
                Vector3 direction = (agent.playerTransform.position - agent.navMeshAgent.destination);
                direction.y = 0;

                if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
                {
                    if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                        agent.navMeshAgent.destination = agent.playerTransform.position;
                }

                timer = agent.config.maxTime;
            }
        }
    }*/
}