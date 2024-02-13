using RatGamesStudios.OperationDeratization.Player;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy.State
{
    public class AiIdleState : AiState
    {
        public AiStateId GetId()
        {
            return AiStateId.Idle;
        }
        public void Enter(AiAgent agent)
        {
            agent.weapons.DeactiveWeapon();
            agent.navMeshAgent.ResetPath();
        }
        public void Update(AiAgent agent)
        {
            
        }
        public void Exit(AiAgent agent)
        {

        }
    }
}