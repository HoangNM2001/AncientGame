using System;
using DG.Tweening;
using Pancake;
using UnityEngine;

public class ResourceFlyUI : GameComponent
{
    private RectTransform rectTransform;
    private float animationDuration;
    private float midPointRatio;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        midPointRatio = 0.5f;
    }

    public void DoMove(Vector3 endPoint, Action completeAction)
    {
        animationDuration = UnityEngine.Random.Range(0.5f, 1.5f);
        rectTransform.localScale = Vector3.one;
        
        var position = rectTransform.position;
        var midPoint = (1 - midPointRatio) * position + midPointRatio * endPoint;
        
        Vector3[] path = { position, midPoint, endPoint };
        
        rectTransform.DOPath(path, animationDuration, PathType.CatmullRom)
            .SetEase(Ease.InQuad);
        
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