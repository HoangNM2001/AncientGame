using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShowableUI : MonoBehaviour
{
    private Vector3 _defaultUIScale;
    private const float ShowDuration = 0.25f;

    private void Start()
    {
        _defaultUIScale = transform.localScale;
        gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
    }

    public void Show(bool isShow)
    {
        if (isShow)
        {
            gameObject.SetActive(true);
            transform.DOScale(_defaultUIScale, ShowDuration);
        }
        else
        {
            transform.DOScale(0.0f, ShowDuration).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
