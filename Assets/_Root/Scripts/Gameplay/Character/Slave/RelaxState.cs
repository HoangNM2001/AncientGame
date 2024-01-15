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
        // Debug.LogError("RelaxState");
        SlaveController.SphereCollider.enabled = false;
        SlaveController.MoveToRelaxPos();
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        SlaveController.Relaxing();
    }

    protected override void OnStateExit(State to)
    {
        SlaveController.SphereCollider.enabled = true;
    }
}
