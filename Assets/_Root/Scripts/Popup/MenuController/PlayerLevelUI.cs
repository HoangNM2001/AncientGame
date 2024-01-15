using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : GameComponent
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expProcessBar;
    [SerializeField] private PlayerLevel playerLevel;

    protected override void OnEnabled()
    {
        playerLevel.OnExpChangedEvent += OnExpChangedEvent;
        playerLevel.OnLevelChangedEvent += OnLevelChangedEvent;
    }

    protected override void OnDisabled()
    {
        playerLevel.OnExpChangedEvent -= OnExpChangedEvent;
        playerLevel.OnLevelChangedEvent -= OnLevelChangedEvent;
    }

    private void Start()
    {
        OnLevelChangedEvent(playerLevel.Level);
        OnExpChangedEvent(playerLevel.Exp);
    }

    private void OnLevelChangedEvent(int value)
    {
        levelText.text = $"level {value}";
    }

    private void OnExpChangedEvent(float value)
    {
        if (playerLevel.IsMaxLevel)
        {
            expProcessBar.fillAmount = 1;
        }
        else
        {
            expProcessBar.fillAmount = value / playerLevel.RequiredExp;
        }
    }

}
