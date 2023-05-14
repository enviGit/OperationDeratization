using UnityEngine;

public class AiIdleState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }

    public void Enter(AiAgent agent)
    {
        
    }
    public void Update(AiAgent agent)
    {
        Vector3 playerDirection = agent.playerTransform.position - agent.transform.position;

        if (playerDirection.magnitude > agent.config.maxSightDistance)
            return;

        Vector3 agentDirection = agent.transform.forward;
        playerDirection.Normalize();
        float dotProduct = Vector3.Dot(playerDirection, agentDirection);

        if (dotProduct > 0)
            agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
    }
    public void Exit(AiAgent agent)
    {
        
    }
}
