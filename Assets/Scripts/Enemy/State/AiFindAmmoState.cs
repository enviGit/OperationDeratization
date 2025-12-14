using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiFindAmmoState : AiState
    {
        private GameObject pickup;
        private GameObject[] pickups = new GameObject[3];
        private float wanderRadius = 10f;

        public AiStateId GetId()
        {
            return AiStateId.FindAmmo;
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
            pickup = FindPickup(agent);

            if (pickup)
                CollectPickup(agent, pickup);
            else
            {
                if (!agent.navMeshAgent.hasPath)
                {
                    Vector3 randomPoint = agent.RandomNavmeshLocation(wanderRadius);
                    agent.navMeshAgent.SetDestination(randomPoint);
                }
            }
            if (!agent.weapons.IsLowAmmo())
                agent.stateMachine.ChangeState(AiStateId.Patrol);
        }
        private GameObject FindPickup(AiAgent agent)
        {
            int count = agent.sightSensor.Filter(pickups, "Interactable", "AmmoBox");

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