using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaxState : SlaveBaseState
{
    public RelaxState(SlaveController slaveController) : base(slaveController)
    {
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        SlaveController.Relaxing();
    }
}
