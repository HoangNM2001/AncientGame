using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlaveBaseState : State
{
    protected SlaveController SlaveController;

    public SlaveBaseState(SlaveController slaveController) : base()
    {
        SlaveController = slaveController;
    }
}
