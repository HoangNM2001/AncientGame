using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaveResourcesUI : MonoBehaviour
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI fillText;

    private int currentCapacity = 0;
    private int maxCapacity;

    public void Setup(Sprite resourceSprite, int capacity)
    {
        resourceIcon.sprite = resourceSprite;
        resourceIcon.SetNativeSize();

        maxCapacity = capacity;
    }

    public void UpdateCapacity(int currCapacity, Action callback = null)
    {
        fillBar.DOFillAmount((float)currCapacity / maxCapacity, 0.5f).OnComplete(() =>
        {
            callback?.Invoke();
        });
        
        DOTween.To(() => currentCapacity, x => currentCapacity = x, currCapacity, 0.5f).SetUpdate(true)
            .OnUpdate(() => fillText.text = $"{currentCapacity}/{maxCapacity}");
    }
}