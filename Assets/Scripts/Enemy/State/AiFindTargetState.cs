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
            //Wander
            if (!agent.navMeshAgent.hasPath)
            {
                Vector3 randomPoint = RandomNavmeshLocation(wanderRadius, agent);
                agent.navMeshAgent.SetDestination(randomPoint);
            }
            if (agent.targeting.HasTarget)
            {
                agent.stateMachine.ChangeState(AiStateId.AttackTarget);
                timeSinceLastSawTarget = 0f; // Reset timer
            }
            else
            {
                timeSinceLastSawTarget += Time.deltaTime;

                if (timeSinceLastSawTarget >= maxTimeWithoutTarget)
                {
                    agent.stateMachine.ChangeState(AiStateId.Patrol);
                    timeSinceLastSawTarget = 0f;
                }
            }
        }
        public void Exit(AiAgent agent)
        {

        }
        private Vector3 RandomNavmeshLocation(float radius, AiAgent agent)
        {
            // Find the closest target location
            CriticalLocations closestLocation = null;
            float closestDistance = float.MaxValue;

            foreach (CriticalLocations location in agent.locations)
            {
                float distance = Vector3.Distance(agent.transform.position, location.location.position);

                if (distance < closestDistance)
                {
                    closestLocation = location;
                    closestDistance = distance;
                }
            }

            // If a valid location is found, select a random point within its radius
            if (closestLocation != null)
            {
                Vector3 randomDirection = Random.insideUnitSphere * closestLocation.radius;
                randomDirection += closestLocation.location.position;
                NavMeshHit navHit;
                NavMesh.SamplePosition(randomDirection, out navHit, closestLocation.radius, NavMesh.AllAreas);

                return navHit.position;
            }
            else
            {
                // No valid target location found, return a random point within the entire radius
                Vector3 randomDirection = Random.insideUnitSphere * radius;
                randomDirection += agent.transform.position;
                NavMeshHit navHit;
                NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas);

                return navHit.position;
            }
        }
    }
}