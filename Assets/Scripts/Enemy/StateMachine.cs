using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    public PatrolState patrolState;

    public void Initialise()
    {
        patrolState = new PatrolState();
        ChangeState(patrolState);
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if(activeState != null)
            activeState.Perform();
    }
    public void ChangeState(BaseState newState)
    {
        if (activeState != null)
            activeState.Exit();

        activeState = newState;

        if (activeState != null)
        {
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
            
    }
}
