using System.Collections;
using System.Collections.Generic;
using Pancake.Threading.Tasks.Triggers;
using UnityEngine;

public class FarmingState : SlaveBaseState
{
    public FarmingState(SlaveController slaveController) : base(slaveController)
    {
    }

    protected override void OnStateEnter(State from, object data)
    {
        SlaveController.ActiveFarmAction();
        SlaveController.GoToNearestField();
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        SlaveController.Farming();
    }
}
