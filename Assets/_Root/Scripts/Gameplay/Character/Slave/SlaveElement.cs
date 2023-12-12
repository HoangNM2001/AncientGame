using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class SlaveElement : SaveDataElement
{
    [SerializeField] private GameObject drownSlave;
    [SerializeField] private SlaveController unlockedSlave;
    [SerializeField] private int price;
    [SerializeField] private Trigger triggerHelp;

    public int Price => price;
    private bool IsUnlock
    {
        get => Data.Load(uniqueId + "_Unlock", false);
        set => Data.Save(uniqueId + "_Unlock", value);
    }

    private void Awake()
    {
        IsUnlock = false;
    }

    protected override void OnEnabled()
    {
        triggerHelp.EnterTriggerEvent += TriggerHelp;
        triggerHelp.ExitTriggerEvent += ExitTriggerHelp;
    }

    protected override void OnDisabled()
    {
        triggerHelp.EnterTriggerEvent -= TriggerHelp;
        triggerHelp.ExitTriggerEvent -= ExitTriggerHelp;
    }

    private void Start()
    {
        SetSlaveElementState();
    }

    public void UnlockSlave()
    {
        IsUnlock = true;
        SetSlaveElementState();
    }

    private void SetSlaveElementState()
    {
        drownSlave.SetActive(!IsUnlock);
        unlockedSlave.gameObject.SetActive(IsUnlock);
    }

    private void TriggerHelp(Collider collider)
    {
        if (collider.TryGetComponent<CharacterHandleTrigger>(out var player))
        {
            player.TriggerSaveSlave(gameObject);
        }
    }

    private void ExitTriggerHelp(Collider collider)
    {
        if (collider.TryGetComponent<CharacterHandleTrigger>(out var player))
        {
            player.ExitTriggerAction();
        }
    }
}
