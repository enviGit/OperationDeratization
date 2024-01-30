using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiFindFirstAidKitState : AiState
    {
        private GameObject pickup;
        private GameObject[] pickups = new GameObject[3];
        private float wanderRadius = 10f;

        public AiStateId GetId()
        {
            return AiStateId.FindFirstAidKit;
        }
        public void Enter(AiAgent agent)
        {
            pickup = null;
            agent.navMeshAgent.speed = agent.config.findWeaponSpeed;
        }
        public void Exit(AiAgent agent)
        {

        }
        public void Update(AiAgent agent)
        {
            // Find pickup
            pickup = FindPickup(agent);

            if (pickup)
                CollectPickup(agent, pickup);
            else
            {
                // Wander if no pickup is found
                if (!agent.navMeshAgent.hasPath)
                {
                    Vector3 randomPoint = RandomNavmeshLocation(wanderRadius, agent);
                    agent.navMeshAgent.SetDestination(randomPoint);
                }
            }
            //Wander
            if (!agent.navMeshAgent.hasPath)
            {
                Vector3 randomPoint = RandomNavmeshLocation(wanderRadius, agent);
                agent.navMeshAgent.SetDestination(randomPoint);
            }
            if (!agent.health.IsLowHealth())
                agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
        private Vector3 RandomNavmeshLocation(float radius, AiAgent agent)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += agent.transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas);

            return navHit.position;
        }
        private GameObject FindPickup(AiAgent agent)
        {
            int count = agent.sightSensor.Filter(pickups, "Interactable", "FirstAidKit");

            GameObject closestPickup = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                float distance = Vector3.Distance(agent.transform.position, pickups[i].transform.position);

                if (distance < closestDistance)
                {
                    closestPickup = pickups[i];
                    closestDistance = distance;
                }
            }

            return closestPickup;
        }
        private void CollectPickup(AiAgent agent, GameObject pickup)
        {
            if (agent.sightSensor.Objects.Contains(pickup))
                agent.navMeshAgent.destination = pickup.transform.position;
        }
    }
}