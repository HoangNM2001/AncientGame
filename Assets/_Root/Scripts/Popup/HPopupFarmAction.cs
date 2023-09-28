using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake.UI;
using UnityEditor;
using UnityEngine;

public class HPopupFarmAction : UIPopup
{
    [SerializeField] private ScriptableListInt fieldStateList;
    [SerializeField] private GameObject seedButton;
    [SerializeField] private GameObject waterButton;
    [SerializeField] private GameObject harvestButton;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        seedButton.SetActive(fieldStateList.Contains((int)EnumPack.FieldState.Seedale));
        waterButton.SetActive(fieldStateList.Contains((int)EnumPack.FieldState.Waterable));
        harvestButton.SetActive(fieldStateList.Contains((int)EnumPack.FieldState.Harvestable));
    }
}
