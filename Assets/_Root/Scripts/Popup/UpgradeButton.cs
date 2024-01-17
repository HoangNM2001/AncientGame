using System;
using Pancake;
using Pancake.UI;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : GameComponent
{
    [SerializeField] private Upgradable upgradable;
    // [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private UIButton button;
    [SerializeField] private Image enableBtnBg;
    [SerializeField] private Image disableBtnBg;
    [SerializeField] private TextMeshProUGUI buttonText;

    protected override void OnEnabled()
    {
        upgradable.OnUpgraded += OnUpgraded;
        
        background.sprite = upgradable.background;
        icon.sprite = upgradable.icon;
        
        OnUpgraded();
    }

    protected override void OnDisabled()
    {
        upgradable.OnUpgraded -= OnUpgraded;
    }

    private void OnUpgraded()
    {
        SetStat();
        SetButtonStatus();
    }

    private void SetStat()
    {
        levelText.text = $"Level {upgradable.Level}";
        descText.text = $"{upgradable.desc}: {(upgradable.descMultiline ? "<br>":"")}{upgradable.Value}";
        var cost = upgradable.Cost > 1000 ? upgradable.Cost.ToString("0.#") : upgradable.Cost.ToString("0");
        // if (upgradable.IsMaxLevel)
        // {
        //     buttonText.text = "MAX";
        // }
        // else
        // {
        //     buttonText.text = upgradable.costType == EnumPack.CostType.Coin ? cost : "UPGRADE";
        // }
        buttonText.text = upgradable.IsMaxLevel ? "MAX" : upgradable.costType == EnumPack.CostType.Coin ? $"{cost} <sprite=0>" : "UPGRADE";
        // nameText.text = upgradable.name;
    }

    private void SetButtonStatus()
    {
        button.interactable = upgradable.IsUpgradable;
        enableBtnBg.gameObject.SetActive(upgradable.IsUpgradable);
        disableBtnBg.gameObject.SetActive(!upgradable.IsUpgradable);
    }
}
