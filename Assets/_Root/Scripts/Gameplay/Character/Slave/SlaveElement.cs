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
    [SerializeField] private List<ExtendField> extendFieldList;

    public int Price => price;
    private bool IsUnlock
    {
        get => Data.Load(Id + "_Unlock", false);
        set => Data.Save(Id + "_Unlock", value);
    }

    private void Awake()
    {
        Initialize();
    }

    protected override void OnEnabled()
    {
        triggerHelp.EnterTriggerEvent += TriggerHelp;
        triggerHelp.ExitTriggerEvent += ExitTriggerHelp;
        foreach (var extendField in extendFieldList)
        {
            extendField.OnActivate += SetSlaveElementState;
        }
    }

    protected override void OnDisabled()
    {
        triggerHelp.EnterTriggerEvent -= TriggerHelp;
        triggerHelp.ExitTriggerEvent -= ExitTriggerHelp;
        foreach (var extendField in extendFieldList)
        {
            extendField.OnActivate -= SetSlaveElementState;
        }
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

    private bool CheckUnlockable()
    {
        foreach (var extendField in extendFieldList)
        {
            if (!extendField.IsUnlocked) return false;
        }
        return true;
    }

    private void SetSlaveElementState()
    {
        if (CheckUnlockable())
        {
            drownSlave.SetActive(!IsUnlock);
            unlockedSlave.gameObject.SetActive(IsUnlock);
            foreach (var extendField in extendFieldList)
            {
                extendField.OnActivate -= SetSlaveElementState;
            }
        }
        else
        {
            drownSlave.SetActive(false);
            unlockedSlave.gameObject.SetActive(false);
        }
    }

    private void TriggerHelp(Collider collider)
    {
        if (!CheckUnlockable()) return;
        if (collider.TryGetComponent<CharacterHandleTrigger>(out var player))
        {
            player.TriggerSaveSlave(gameObject);
        }
    }

    private void ExitTriggerHelp(Collider collider)
    {
        if (!CheckUnlockable()) return;
        if (collider.TryGetComponent<CharacterHandleTrigger>(out var player))
        {
            player.ExitTriggerAction();
        }
    }
}
