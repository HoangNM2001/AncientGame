using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaxState : SlaveBaseState
{
    public RelaxState(SlaveController slaveController) : base(slaveController)
    {
    }

    protected override void OnStateEnter(State from, object data)
    {
        Debug.LogError("RelaxState");
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        SlaveController.Relaxing();
    }
}
