using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlaveBaseState : State
{
    protected SlaveController SlaveController;

    protected SlaveBaseState(SlaveController slaveController)
    {
        SlaveController = slaveController;
    }
}
