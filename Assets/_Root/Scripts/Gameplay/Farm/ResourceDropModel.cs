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
    
    private const float ForceMin = 0.5f;
    private const float ForceMax = 2;
    private const float FloorHeight = 0.0f;
    private const float SleepThreshold = 0.05f;
    private const float ReflectPower = 0.5f;
    private const float Gravity = -9.8f;
    
    protected override void OnEnabled()
    {
        isStopped = false;
        var randomDir = Random.insideUnitCircle.normalized;
        randomBounceTime = Random.Range(1, 2);
        velocity = new Vector3(randomDir.x, 2.0f, randomDir.y) * Random.Range(ForceMin, ForceMax);
    }

    protected override void LateTick()
    {
        if (isStopped) return;
        if (velocity.magnitude > SleepThreshold || transform.position.y > FloorHeight)
        {
            velocity.y += Gravity * Time.fixedDeltaTime * dropSpeed;
        }
        
        transform.position += velocity * Time.fixedDeltaTime;
        
        if (transform.position.y <= FloorHeight) 
        {
            transform.position = new Vector3(transform.position.x, FloorHeight, transform.position.z);
            if (randomBounceTime > 0)
            {
                velocity.y = -velocity.y;
                velocity *= ReflectPower;
                randomBounceTime--;
            }
            else
            {
                isStopped = true;
            }
        }
    }
}