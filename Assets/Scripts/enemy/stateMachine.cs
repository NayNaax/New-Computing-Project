using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stateMachine : MonoBehaviour
{   
    public baseState currentState;
    public patrolState patrolState;

    public void Initialise()
    {
        patrolState = new patrolState();
        ChangeState(patrolState);
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
    
    public void ChangeState(baseState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        
        currentState = newState;

        // fail safe null check to make sure the new state is not null
        if (currentState != null)
        {
            // enter the new state
            currentState.stateMachine = this;
            currentState.enemy = GetComponent<enemy>();
            currentState.Enter();
        }
    }
}
