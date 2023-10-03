using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CharacterAnimController : GameComponent
{
    [Header("BASE")]
    private Animator animator;
    private string animationName;

    [Header("EVENT")]
    [SerializeField] private ScriptableEventNoParam dropSeedEvent;
    [SerializeField] private ScriptableEventNoParam waterFarmEvent;
    [SerializeField] private ScriptableEventNoParam harvestFruitEvent;

    public string AnimationName => animationName;
    public float Speed
    {
        get => animator.speed;
        set => animator.speed = value;
    }
    
    private const string VelocityParamStr = "Velocity";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateIdle2Run(float velocity, float deltaTime)
    {
        animator.SetFloat(VelocityParamStr, velocity, 0.1f, deltaTime);
    }

    public void Play(string animName, int layer, bool fade = true)
    {
        animationName = animName;

        if (!fade)
        {
            animator.Play(animationName, layer);
        }
        else
        {
            animator.CrossFade(animationName, 0.1f, layer);
        }
    }

    public void TriggerSeedFarm()
    {
        dropSeedEvent.Raise();
    }

    public void TriggerWaterFarm()
    {
        waterFarmEvent.Raise();
    }

    public void TriggerHarvestFruit()
    {
        harvestFruitEvent.Raise();        
    }
}
