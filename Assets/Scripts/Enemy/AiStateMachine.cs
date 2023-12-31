using System;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class AiStateMachine
    {
        public AiState[] states;
        public AiAgent agent;
        public AiStateId currentState;

        public AiStateMachine(AiAgent agent)
        {
            this.agent = agent;
            int numStates = Enum.GetNames(typeof(AiStateId)).Length;
            states = new AiState[numStates];
        }
        public void RegisterState(AiState state)
        {
            int index = (int)state.GetId();
            states[index] = state;
        }
        public AiState GetState(AiStateId stateId)
        {
            int index = (int)stateId;

            return states[index];
        }
        public void Update()
        {
            GetState(currentState)?.Update(agent);
        }
        public void ChangeState(AiStateId newState)
        {
            GetState(currentState)?.Exit(agent);
            currentState = newState;
            GetState(currentState)?.Enter(agent);
        }
    }
}