using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pancake;
using DG.Tweening;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System;

public class TileLand : GameComponent
{
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject landType1, landType2, landType3, landType4, landType5;
    [SerializeField] private Trigger triggerLeft, triggerRight, triggerUp, triggerDown;
    [SerializeField] private TileLand landLeft, landRight, landUp, landDown;

    private bool isLeft, isRight, isUp, isDown;
    private bool isUpdated;
    private Vector3 modelDefaultEuler = Vector3.zero;

    private const float animDuration = 0.7f;

    public EnumPack.LandType Type { get; set; }

    private void Awake()
    {
        if (model) modelDefaultEuler = model.transform.eulerAngles;
    }

    public void Activate(bool restore = true, Action callback = null)
    {
        gameObject.SetActive(true);
        UpdateModel();

        DOTween.Kill(transform);

        if (model)
        {
            if (restore)
            {
                model.transform.localScale = Vector3.one;
                OnActivated();
            }
            else
            {
                model.transform.localScale = Vector3.zero;
                model.transform.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack).SetTarget(transform).OnComplete(() =>
                {
                    callback?.Invoke();
                    OnActivated();
                });
            }
        }
        else
        {
            OnActivated();
        }
    }

    public void Unlock()
    {
        model.transform.localScale = Vector3.zero;
        model.transform.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack).SetTarget(transform);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        DOTween.Kill(transform);
    }

    private void OnActivated()
    {
        StartCoroutine(UpdateObjectNear());
    }

    public void UpdateModel()
    {
        if (model != null)
        {
            landType1.gameObject.SetActive(false);
            landType2.gameObject.SetActive(false);
            landType3.gameObject.SetActive(false);
            landType4.gameObject.SetActive(false);
            landType5.gameObject.SetActive(false);
            isLeft = isRight = isUp = isDown = false;
            landLeft = landRight = landUp = landDown = null;
            isUpdated = false;

            var listLeft = triggerLeft.GetObjectNear();
            foreach (var obj in listLeft)
            {
                if (obj != gameObject && obj.TryGetComponent<TileLand>(out var land))
                {
                    landLeft = land;
                    isLeft = true;
                }
            }

            var listRight = triggerRight.GetObjectNear();
            foreach (var obj in listRight)
            {
                if (obj != gameObject && obj.TryGetComponent<TileLand>(out var land))
                {
                    landRight = land;
                    isRight = true;
                }
            }

            var listUp = triggerUp.GetObjectNear();
            foreach (var obj in listUp)
            {
                if (obj != gameObject && obj.TryGetComponent<TileLand>(out var land))
                {
                    landUp = land;
                    isUp = true;
                }
            }

            var listDown = triggerDown.GetObjectNear();
            foreach (var obj in listDown)
            {
                if (obj != gameObject && obj.TryGetComponent<TileLand>(out var land))
                {
                    landDown = land;
                    isDown = true;
                }
            }

            var side = 0;
            if (isLeft) side++;
            if (isRight) side++;
            if (isUp) side++;
            if (isDown) side++;

            if (side == 1)
            {
                Type = EnumPack.LandType.Type4;
                if (isLeft) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 0);
                if (isRight) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 180);
                if (isUp) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 90);
                if (isDown) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 270);
            }
            else if (side == 2)
            {
                if ((isUp && isDown) || (isLeft && isRight))
                {
                    Type = EnumPack.LandType.Type5;
                    if (isUp && isDown) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 0);
                    if (isLeft && isRight) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 90);
                }
                else
                {
                    Type = EnumPack.LandType.Type3;

                    if (isLeft && isUp) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 0);
                    if (isLeft && isDown) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 270);
                    if (isUp && isRight) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 90);
                    if (isRight && isDown) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 180);
                }
            }
            else if (side == 3)
            {
                Type = EnumPack.LandType.Type2;
                if (!isLeft) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 180);
                if (!isRight) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 0);
                if (!isUp) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 270);
                if (!isDown) model.transform.localEulerAngles = new Vector3(modelDefaultEuler.x, modelDefaultEuler.y, 90);
            }
            else if (side == 4)
            {
                Type = EnumPack.LandType.Type1;
            }

            switch (Type)
            {
                case EnumPack.LandType.Type1:
                    landType1.gameObject.SetActive(true);
                    break;
                case EnumPack.LandType.Type2:
                    landType2.gameObject.SetActive(true);
                    break;
                case EnumPack.LandType.Type3:
                    landType3.gameObject.SetActive(true);
                    break;
                case EnumPack.LandType.Type4:
                    landType4.gameObject.SetActive(true);
                    break;
                case EnumPack.LandType.Type5:
                    landType5.gameObject.SetActive(true);
                    break;
            }

            isUpdated = true;

        }
    }

    IEnumerator UpdateObjectNear()
    {
        yield return new WaitUntil(() => isUpdated);
        landLeft?.UpdateModel();
        landRight?.UpdateModel();
        landUp?.UpdateModel();
        landDown?.UpdateModel();
    }
}
