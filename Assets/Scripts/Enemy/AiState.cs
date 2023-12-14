public enum AiStateId
{
    ChasePlayer,
    Death,
    Idle,
    FindWeapon,
    AttackTarget,
    FindTarget
}
public interface AiState
{
    AiStateId GetId();

    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}