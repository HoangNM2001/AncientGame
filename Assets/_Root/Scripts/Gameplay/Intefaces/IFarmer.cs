using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFarmer
{
    public void TriggerActionFarm(GameObject gameObject = null);
    public void ExitTriggerActionFarm();
}
