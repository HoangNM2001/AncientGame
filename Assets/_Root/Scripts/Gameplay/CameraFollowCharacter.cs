using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CameraFollowCharacter : GameComponent
{
    [SerializeField] private Transform mainCameraTrans;
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;

    [Header("Camera Move")] 
    [SerializeField] private Vector3 moveCamPosition;
    [SerializeField] private Vector3 moveCamRotation;
    
    [Header("Camera ShakeTree")]
    [SerializeField] private Vector3 shakeTreeCamPosition;
    [SerializeField] private Vector3 shakeTreeCamRotation;

    private Transform characterTrans;
    private Vector3 velocity;
    private const float SmoothTime = 0.3f;
    private EnumPack.ControlType controlType;

    protected override void OnEnabled()
    {
        changeInputEvent.OnRaised += changeInputEvent_OnRaised;
    }

    private void changeInputEvent_OnRaised(int newControlType)
    {
        controlType = (EnumPack.ControlType)newControlType;
        ChangeCamera();
    }

    protected override void OnDisabled()
    {
        changeInputEvent.OnRaised -= changeInputEvent_OnRaised;
    }

    private void Start()
    {
        characterTrans = getCharacterEvent.Raise().transform;
        
        controlType = EnumPack.ControlType.Move;
        ChangeCamera();
    }

    protected override void LateTick()
    {
        if (!characterTrans) return;
        if (controlType != EnumPack.ControlType.Move) return;
        
        transform.position = Vector3.SmoothDamp(transform.position, characterTrans.transform.position, ref velocity, SmoothTime);
    }

    private void ChangeCamera()
    {
        switch (controlType)
        {
            case EnumPack.ControlType.Move:
                MoveCamera(moveCamPosition, moveCamRotation);
                break;
            case EnumPack.ControlType.Horizontal:
                MoveCamera(shakeTreeCamPosition, shakeTreeCamRotation);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MoveCamera(Vector3 newPosition, Vector3 newRotation)
    {
        mainCameraTrans.localPosition = newPosition;
        mainCameraTrans.localRotation = Quaternion.Euler(newRotation);
    }

#if UNITY_EDITOR
    [ContextMenu("Camera Move")]
    public void CameraMove()
    {
        MoveCamera(moveCamPosition, moveCamRotation);
    }
    
    [ContextMenu("Camera Shake")]
    public void CameraShake()
    {
        MoveCamera(shakeTreeCamPosition, shakeTreeCamRotation);
    }
#endif
}
