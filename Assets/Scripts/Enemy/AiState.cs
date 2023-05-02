public enum AiStateId
{
    ChasePlayer,
    Death,
    Idle
}
public interface AiState
{
    AiStateId GetId();

    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}