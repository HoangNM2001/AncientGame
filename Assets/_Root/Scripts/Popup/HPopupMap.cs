using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupMap : UIPopup
{
    [SerializeField] private PlayerControllerVariable playerVariable;
    [SerializeField] private ScriptableListTile tileList;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private TileView tileViewPrefab;
    [SerializeField] private Transform mapParent;
    [SerializeField] private Transform playerPos;

    private List<TileView> _tileViewList;

    private void Awake()
    {
        _tileViewList = new List<TileView>();

        for (var i = 0; i < tileList.Count; i++)
        {
            var tileView = Instantiate(tileViewPrefab, mapParent);
            _tileViewList.Add(tileView);
            // Debug.LogError(tileList[i].Coord);
        }
    }

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        toggleMenuUIEvent.Raise(false);

        for (var i = 0; i < _tileViewList.Count; i++)
        {
            _tileViewList[i].Initialize(tileList[i]);
        }

        var index = SimpleMath.GetNearestIndex(playerVariable.Value.transform.position, tileList.Select(t => new Vector3(t.transform.position.x, 0.0f, t.transform.position.z)).ToArray());

        playerPos.transform.localPosition = _tileViewList[index].transform.localPosition;
        playerPos.transform.SetAsLastSibling();
    }

    protected override void OnAfterClose()
    {
        base.OnAfterClose();
        toggleMenuUIEvent.Raise(true);
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
