using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Unity.Android.Types;
using UnityEngine;

public class ResourceFlyUI : GameComponent
{
    private RectTransform rectTransform;
    private float animationDuration;
    private float midPointRatio;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animationDuration = 1.0f;
        midPointRatio = 0.5f;
    }

    public void DoMove(Vector3 endPoint, Action completeAction)
    {
        rectTransform.localScale = Vector3.one;

        // Calculate the curve's highest point as the midpoint of the start and end, raised in Y.
        var position = rectTransform.position;
        
        // var midPoint = (position + endPoint) * midPointRatio;
        var midPoint = (1 - midPointRatio) * position + midPointRatio * endPoint;
        // midPoint += new Vector3(0, 5, 0); // Adjust this to control the height of the curve's highest point.

        // Creating a path
        Vector3[] path = { position, midPoint, endPoint };

        // Animate position along the curve
        rectTransform.DOPath(path, animationDuration, PathType.CatmullRom)
            .SetEase(Ease.InQuad);

        // Scale animation
        DOTween.To(() => rectTransform.localScale,
                x => rectTransform.localScale = x,
                new Vector3(1.5f, 1.5f, 1.5f), animationDuration * midPointRatio)
            .OnComplete(() =>
            {
                DOTween.To(() => rectTransform.localScale,
                    x => rectTransform.localScale = x,
                    Vector3.one, animationDuration * (1 - midPointRatio)).OnComplete(() => completeAction?.Invoke());
            });
    }
}