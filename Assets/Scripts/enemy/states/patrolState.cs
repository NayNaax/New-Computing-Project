using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolState : baseState
{
    public int waypointIndex;
    public override void Enter()
    {
    }

    public override void Execute()
    {
        patrolCycle();
    }

    public override void Exit()
    {
    }

    public void patrolCycle()
    {
        // implement patrol cycle
        if (enemy.Agent.remainingDistance < 0.2f)
        {
            // check if the waypoint index is less than the number of waypoints
            if (waypointIndex < enemy.path.waypoints.Count - 1)
            {
                // increment the waypoint index
                waypointIndex++;
            }
            else
            {
                // reset the waypoint index
                waypointIndex = 0;
            }
            // set the destination to the next waypoint
            enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
        }
    }
}

