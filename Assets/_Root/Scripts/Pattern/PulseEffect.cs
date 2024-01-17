using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using UnityEngine;

public class PulseEffect : GameComponent
{
    [SerializeField] private float scale = 1.2f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private int loopCount = -1;
    [SerializeField] private bool from;

    Vector3 originalScale;

    protected override void OnEnabled()
    {
        originalScale = transform.localScale;
        DOTween.Kill(this);
        var tween = transform.DOScale(scale * originalScale, duration);
        if (from)
        {
            tween = tween.From();
        }
        if (loopCount != 0)
        {
            tween = tween.SetLoops(loopCount, LoopType.Yoyo);   
        }
        tween.SetEase(ease).SetTarget(this);
        tween.Play();
    }

    protected override void OnDisabled()
    {
        DOTween.Kill(this);
        transform.localScale = originalScale;
    }
}
