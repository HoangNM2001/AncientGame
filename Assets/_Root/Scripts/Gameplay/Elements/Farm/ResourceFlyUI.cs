using System;
using DG.Tweening;
using Pancake;
using UnityEngine;

public class ResourceFlyUI : GameComponent
{
    protected RectTransform rectTransform;
    protected float animationDuration;
    protected float midPointRatio;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animationDuration = 1.0f;
        midPointRatio = 0.5f;
    }

    public virtual void DoMove(Vector3 endPoint, Action completeAction)
    {
        animationDuration = UnityEngine.Random.Range(0.5f, 1.5f);
        rectTransform.localScale = Vector3.one;

        // Calculate the curve's highest point as the midpoint of the start and end, raised in Y.
        var position = rectTransform.position;

        // var midPoint = (position + endPoint) * midPointRatio;
        var midPoint = (1 - midPointRatio) * position + midPointRatio * endPoint;
        // midPoint += new Vector3(0, 5, 0); // Adjust this to control the height of the curve's highest point.

        // var sin = (endPoint.y - position.y) / (endPoint.x - endPoint.x);
        // var cos = Mathf.Sqrt(1 - sin * sin);
        // var randomSign = UnityEngine.Random.Range(0, 2) * 2 - 1;
        // var randomOffset = UnityEngine.Random.Range(-Vector3.Distance(midPoint, position), Vector3.Distance(midPoint, position));
        // midPoint.x -= randomSign * randomOffset * sin;
        // midPoint.y += randomSign * randomOffset * cos;

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