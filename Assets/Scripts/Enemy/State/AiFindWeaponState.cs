using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiFindWeaponState : AiState
    {
        private GameObject pickup;
        private GameObject[] pickups = new GameObject[3];
        private float wanderRadius = 10f;

        public AiStateId GetId()
        {
            return AiStateId.FindWeapon;
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
            if (agent.weapons.HasWeapon())
                agent.stateMachine.ChangeState(AiStateId.Patrol);
        }
        private GameObject FindPickup(AiAgent agent)
        {
            int count = agent.sightSensor.Filter(pickups, "Interactable", "Weapon");

            if (count > 0)
                return pickups[0];

            return null;
        }
        private void CollectPickup(AiAgent agent, GameObject pickup)
        {
            agent.navMeshAgent.destination = pickup.transform.position;
        }
    }
}