using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Pancake;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceDropModel : GameComponent
{
    [SerializeField] private float dropSpeed;

    private float randomBounceTime;
    private bool isStopped;
    private Vector3 velocity;
    
    private Sequence moveSequence;
    private const float MoveDistance = 0.15f;
    private const float MoveDuration = 1.0f;
    
    private const float ForceMin = 0.5f;
    private const float ForceMax = 2.0f;
    private const float FloorHeight = 0.0f;
    private const float SleepThreshold = 0.05f;
    private const float ReflectPower = 0.5f;
    private const float Gravity = -9.8f;
    private const float RotateDuration = 10.0f;
    private readonly Vector3 rotateBy = new Vector3(0, 360, 0);

    protected override void OnEnabled()
    {
        isStopped = false;
        var randomDir = Random.insideUnitCircle.normalized;
        randomBounceTime = Random.Range(1, 2);
        velocity = new Vector3(randomDir.x, 2.0f, randomDir.y) * Random.Range(ForceMin, ForceMax);
    }

    protected override void OnDisabled()
    {
        DOTween.Kill(this);
        moveSequence.Kill();
    }

    protected override void LateTick()
    {
        if (isStopped) return;
        
        DoDrop();
    }

    private void DoIdle()
    {
        transform.DOLocalRotate(rotateBy, RotateDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear) // Constant rotation speed
            .SetLoops(-1, LoopType.Incremental).SetTarget(this);

        var startPos = transform.position;
        var endPos = startPos + new Vector3(0.0f, MoveDistance, 0.0f);
        
        moveSequence = DOTween.Sequence();

        moveSequence.Append(transform.DOMove(endPos, MoveDuration).SetEase(Ease.InOutSine));
        moveSequence.Append(transform.DOMove(startPos, MoveDuration).SetEase(Ease.InOutSine));

        moveSequence.SetLoops(-1);

        moveSequence.Play();
    }

    private void DoDrop()
    {
        if (velocity.magnitude > SleepThreshold || transform.position.y > FloorHeight)
        {
            velocity.y += Gravity * Time.fixedDeltaTime * dropSpeed;
        }
        
        transform.position += velocity * Time.fixedDeltaTime;

        if (!(transform.position.y <= FloorHeight)) return;

        var position = transform.position;
        position = new Vector3(position.x, FloorHeight, position.z);
        transform.position = position;
        
        if (randomBounceTime > 0)
        {
            velocity.y = -velocity.y;
            velocity *= ReflectPower;
            randomBounceTime--;
        }
        else
        {
            isStopped = true;
            DoIdle();
        }
    }
}