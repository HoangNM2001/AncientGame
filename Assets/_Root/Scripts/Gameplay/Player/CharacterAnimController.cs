using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class CharacterAnimController : GameComponent
{
    private Animator animator;
    private string animationName;

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
}
