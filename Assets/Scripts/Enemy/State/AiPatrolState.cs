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
                agent.stateMachine.ChangeState(AiStateId.AttackTarget);
            if (agent.weapons.IsLowAmmo())
                agent.stateMachine.ChangeState(AiStateId.FindAmmo);
            if (agent.health.IsLowHealth())
                agent.stateMachine.ChangeState(AiStateId.FindFirstAidKit);
        }
        public void Exit(AiAgent agent)
        {

        }
        private Vector3 RandomNavmeshLocation(float radius, AiAgent agent)
        {
            // Find the closest critical location
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
                // No valid location found, return a random point within the entire radius
                Vector3 randomDirection = Random.insideUnitSphere * radius;
                randomDirection += agent.transform.position;
                NavMeshHit navHit;
                NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas);

                return navHit.position;
            }
        }
    }
}