using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    public Action<Vector3> OnShowHitTargetEvent;

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

        if (currentHp <= 0)
        {
            animController.Play(Constant.PREDATOR_DIE, 0);
        }
    }

    public void Hurt(Transform transform)
    {
        if (cacheBlood == null)
        {
            cacheBlood = Instantiate(bloodFx, transform);
        }
        else
        {
            cacheBlood.transform.SetParent(transform);
            cacheBlood.Play();
        }
        animController.Play(Constant.PREDATOR_HITTED, 0);
    }

    protected override void Tick()
    {
        animController.UpdateIdle2Run(speedRatio, Time.deltaTime);
    }
}
