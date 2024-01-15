using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pancake;
using TMPro;
using System;
using DG.Tweening;

public class FlyText : MonoBehaviour
{
    [SerializeField] private GameObjectPool flyTextPool;

    private Vector3 position;
    private TextMeshPro flyText;

    private void Awake()
    {
        position = Vector3.zero;
        flyText = GetComponent<TextMeshPro>();
    }

    public void Initialize(string text)
    {
        flyText.text = text;

        position = transform.localPosition;
        gameObject.SetActive(true);
    }

    public void Show(bool isActive = false, Action completeAction = null)
    {
        DOTween.Kill(transform);

        if (isActive)
        {
            completeAction?.Invoke();
        }
        else
        {
            transform.DOMove(position + Vector3.up, 1f).OnComplete(() =>
            {
                Hide();
                completeAction?.Invoke();
            });
        }
    }

    public void Hide()
    {
        flyTextPool.Return(gameObject);
    }
}
