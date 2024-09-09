using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : FSMState
{
    private Vector3 desPos;
    private Transform[] waypoints;
    private float curRotSpeed = 1f;
    private float curSpeed = 100f;
    private float playerNearRadius;
    private float patrollRadius;
    

    public PatrolState(Transform[] wp, float playerNearRadius, float patrollRadius)
    {
        waypoints = wp;
        stateID = FSMStateID.Patrolling;
        this.playerNearRadius = playerNearRadius;
        this.patrollRadius = patrollRadius;
    }
    public override void CheckTransitionRules(Transform player, Transform npc)
    {
        // Check the distance with player tank
        // When the distance is near, transition to chase state
        if(Vector3.Distance(npc.position, player.position) <= playerNearRadius) {
            Debug.Log("Switch to chase state");
            NPCTankController npcTankController = npc.GetComponent<NPCTankController>();
            if(npcTankController != null)
            {
                npcTankController.SetTransition(Transition.SawPlayer);
            } else
            {
                Debug.LogError("NPCtankController not found in NPC");
            }


        }
    }

    public override void RunState(Transform player, Transform npc)
    {
        // Find another random patrol point if the current point is reached
        if(Vector3.Distance(npc.position, desPos) <= patrollRadius)
        {
            Debug.Log("Reached to the destination point calculating the next point");
            FindNextPoint();
        }

        // Rotate to the target point
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, desPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        // Go foward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    private void FindNextPoint()
    {
        throw new NotImplementedException();
    }
}
