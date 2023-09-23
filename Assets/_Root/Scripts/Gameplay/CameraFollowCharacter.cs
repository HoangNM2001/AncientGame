using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CameraFollowCharacter : GameComponent
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;

    private Transform characterTrans;
    private Vector3 velocity;
    private float smoothTime = 0.3f;

    private void Start()
    {
        characterTrans = getCharacterEvent.Raise().transform;
    }

    protected override void LateTick()
    {
        if (!characterTrans) return;
        
        transform.position = Vector3.SmoothDamp(transform.position, characterTrans.transform.position, ref velocity, smoothTime);
    }
}
