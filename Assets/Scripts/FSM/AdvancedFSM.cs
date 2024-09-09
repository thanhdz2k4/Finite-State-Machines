using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Transition
{
    None = 0, SawPlayer, ReachPlayer, LostPlayer, NoHealth
}

public enum FSMStateID
{
    None = 0, Patrolling, Chasing, Attacking, Dead,
}



public class AdvancedFSM : FSM
{
    private List<FSMState> fsmStates;
    private FSMStateID currentStateID;

    public FSMStateID CurrentStateID
    {
        get
        {
            return currentStateID;
        }
    }

    private FSMState currentState;

    public FSMState CurrentState
    {
        get
        {
            return currentState;
        }
    }
}
