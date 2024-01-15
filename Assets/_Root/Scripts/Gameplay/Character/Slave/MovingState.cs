using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : SlaveBaseState
{
    public MovingState(SlaveController slaveController) : base(slaveController)
    {
    }

    protected override void OnStateEnter(State from, object data)
    {
        // Debug.LogError("MovingState");
        SlaveController.EmptyLayer1();
        SlaveController.ActionList.StopActionEvent();
        SlaveController.MoveToTargetPos();
    }

    // protected override void OnStateUpdate()
    // {
    //     base.OnStateUpdate();
    //     // SlaveController.Moving();
    // }
}
