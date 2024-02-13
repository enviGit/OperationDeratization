namespace RatGamesStudios.OperationDeratization.Enemy
{
    public enum AiStateId
    {
        Death,
        Idle,
        FindWeapon,
        AttackTarget,
        FindTarget,
        FindFirstAidKit,
        FindAmmo,
        Patrol
    }
    public interface AiState
    {
        AiStateId GetId();

        void Enter(AiAgent agent);
        void Update(AiAgent agent);
        void Exit(AiAgent agent);
    }
}