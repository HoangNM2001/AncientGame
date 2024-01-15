using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using UnityEditor;
using UnityEngine;

public class SaveDataElement : GameComponent
{
    [SerializeField, UniqueID] protected string uniqueId;
    [SerializeField] private GameObjectPool flyTextPool;

    protected Vector3 DefaultScale;
    protected const float AnimDuration = 0.7f;

    public Tile Tile { get; set; }

    protected virtual bool IsUnlocked
    {
        get => Data.Load($"{uniqueId}_isUnlocked", false);
        set => Data.Save($"{uniqueId}_isUnlocked", value);
    }

    protected virtual void Initialize()
    {
        DefaultScale = transform.localScale;
    }

    public virtual void Activate(bool restore = true)
    {
        DOTween.Kill(transform);
        gameObject.SetActive(true);

        if (restore)
        {
            transform.localScale = DefaultScale;
        }
        else
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(DefaultScale, AnimDuration).SetEase(Ease.OutBack).SetTarget(transform);
        }
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }

    protected void ShowFlyText(Vector3 showPos, string text, Action completeAction = null)
    {
        var flyText = flyTextPool.Request().GetComponent<FlyText>();
        flyText.transform.position = showPos + Vector3.up * 3.0f;
        flyText.Initialize(text);
        flyText.Show(false, completeAction);
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Unique Id")]
    public void ResetUniqueID()
    {
        Guid guid = Guid.NewGuid();
        uniqueId = guid.ToString();
        EditorUtility.SetDirty(this);
    }
#endif
}
