using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Predator : GameComponent
{
    [SerializeField] private EnumPack.PredatorType predatorType;
    [SerializeField] private CharacterAnimController animController;
    [SerializeField] private Sprite icon;
    [SerializeField] private int maxHp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int maxStep;
    [SerializeField] private List<SphereCollider> colliderList;
    [SerializeField] public ParticleSystem bloodFx;

    private float speedRatio;
    private int currentHp;
    private ParticleSystem cacheBlood;

    public EnumPack.PredatorType PredatorType => predatorType;
    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public int MaxStep => maxStep;
    public Sprite PredatorIcon => icon;
    public Action OnHpChangeEvent;
    public Action<Vector3> OnShowNextHitPointEvent;

    public void Activate()
    {
        currentHp = maxHp;
        speedRatio = 0.0f;
    }

    public void MoveForward(int step, float stepSize, Action onCompleted)
    {
        var pos = transform.position + Vector3.left * step * stepSize;
        StartCoroutine(IEMoveForward(pos, onCompleted));
    }

    IEnumerator IEMoveForward(Vector3 pos, Action onCompleted)
    {
        speedRatio = 1.0f;
        var duration = (pos - transform.position).magnitude / moveSpeed;
        var tween = transform.DOMove(pos, duration);

        yield return new WaitUntil(() => tween == null || !tween.IsPlaying() || tween.IsComplete());
        speedRatio = 0.0f;
        onCompleted?.Invoke();
    }

    public void Attack(Action onCompleted)
    {
        animController.Play(Constant.PREDATOR_ATTACK, 0);
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() => onCompleted?.Invoke());
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        OnHpChangeEvent?.Invoke();

        animController.Play(currentHp <= 0 ? Constant.PREDATOR_DIE : Constant.PREDATOR_HITTED, 0);
    }

    public void Hurt(Transform hitPos)
    {
        if (cacheBlood == null)
        {
            cacheBlood = Instantiate(bloodFx, hitPos);
        }
        else
        {
            cacheBlood.transform.SetParent(hitPos);
            cacheBlood.Play();
        }
    }

    public void ShowNextHitPoint()
    {
        var random = UnityEngine.Random.Range(0, colliderList.Count);
        for (var i = 0; i < colliderList.Count; i++)
        {
            colliderList[i].enabled = i == random;
        }
        OnShowNextHitPointEvent?.Invoke(colliderList[random].bounds.center);
    }

    protected override void Tick()
    {
        animController.UpdateIdle2Run(speedRatio, Time.deltaTime);
    }
}
