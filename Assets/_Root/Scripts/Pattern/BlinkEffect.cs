using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : GameComponent
{
    private float blinkDuration = 0.5f;
    private int blinkCount = 4;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    protected override void OnEnabled()
    {
        Blink();
    }

    private void Blink()
    {
        DOTween.To(() => 1.0f, x => SetAlpha(x), 0.0f, blinkDuration).SetLoops(blinkCount, LoopType.Yoyo).OnComplete(() => gameObject.SetActive(false));
    }

    private void SetAlpha(float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
