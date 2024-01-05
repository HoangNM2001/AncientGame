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

    private Vector3 defaultScale;
    private const float animDuration = 0.7f;

    public Tile Tile { get; set; }

    public virtual bool IsUnlocked
    {
        get => Data.Load(uniqueId + "isUnlocked", false);
        set => Data.Save(uniqueId + "isUnlocked", value);
    }

    protected virtual void Initialize()
    {
        defaultScale = transform.localScale;
    }

    public virtual void Activate(bool restore = true)
    {
        DOTween.Kill(transform);
        gameObject.SetActive(true);

        if (restore)
        {
            transform.localScale = defaultScale;
        }
        else
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(defaultScale, animDuration).SetEase(Ease.OutBack).SetTarget(transform);
        }
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
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
