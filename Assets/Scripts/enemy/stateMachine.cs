using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stateMachine : MonoBehaviour
{   
    public baseState currentState;
    public patrolState patrolState;
    // property for patrol state


    public void Initialise()
    {
        patrolState = new patrolState();
        ChangeState(patrolState);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
    public void ChangeState(baseState newState)
    {
        // check activeState is not null
        if (currentState != null)
        {
            // exit the current state
            currentState.Exit();
        }
        // change to a new state
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
