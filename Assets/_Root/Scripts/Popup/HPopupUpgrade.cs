using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupUpgrade : UIPopup
{
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private ScriptableEventInt changeInputEvent;

    protected override void OnEnabled()
    {
        playerLevel.OnSkillPointChangedEvent += playerLevel_OnSkillPointChangedEvent;
    }

    protected override void OnDisabled()
    {
        playerLevel.OnSkillPointChangedEvent += playerLevel_OnSkillPointChangedEvent;
    }

    private void playerLevel_OnSkillPointChangedEvent(int skillPoint)
    {
        if (skillPoint <= 0 && gameObject.activeSelf)
        {
            closePopupEvent.Raise();
            changeInputEvent.Raise((int)EnumPack.ControlType.Move);
        }
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
