using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFisher
{
    public void TriggerActionFishing(GameObject gameObject = null);
    public void ExitTriggerActionFishing();
}
