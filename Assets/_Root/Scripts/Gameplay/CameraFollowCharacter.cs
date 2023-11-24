using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CameraFollowCharacter : GameComponent
{
    [SerializeField] private Transform mainCameraTrans;
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private ScriptableEventBool toggleMainCamera;

    [Header("Camera Move")]
    [SerializeField] private Vector3 moveCamPosition;
    [SerializeField] private Vector3 moveCamRotation;

    [Header("Camera ShakeTree")]
    [SerializeField] private Vector3 shakeTreeCamPosition;
    [SerializeField] private Vector3 shakeTreeCamRotation;

    private Transform characterTrans;
    private Vector3 velocity;
    private EnumPack.ControlType controlType;
    private const float SmoothTime = 0.3f;
    private const float CamMoveDuration = 1.5f;

    protected override void OnEnabled()
    {
        changeInputEvent.OnRaised += changeInputEvent_OnRaised;
        toggleMainCamera.OnRaised += toggleMainCamera_OnRaised;
    }

    private void changeInputEvent_OnRaised(int newControlType)
    {
        controlType = (EnumPack.ControlType)newControlType;
        ChangeCamera();
    }

    private void toggleMainCamera_OnRaised(bool status)
    {
        mainCameraTrans.gameObject.SetActive(status);
    }

    protected override void OnDisabled()
    {
        changeInputEvent.OnRaised -= changeInputEvent_OnRaised;
        toggleMainCamera.OnRaised -= toggleMainCamera_OnRaised;
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
                break;
        }
    }

    private void MoveCamera(Vector3 newPosition, Vector3 newRotation, bool isSmooth = true)
    {
        if (isSmooth)
        {
            mainCameraTrans.DOLocalMove(newPosition, CamMoveDuration).SetEase(Ease.OutCirc);
            mainCameraTrans.DOLocalRotate(newRotation, CamMoveDuration).SetEase(Ease.OutCirc);
        }
        else
        {
            mainCameraTrans.localPosition = newPosition;
            mainCameraTrans.localRotation = Quaternion.Euler(newRotation);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Camera Move")]
    public void CameraMove()
    {
        MoveCamera(moveCamPosition, moveCamRotation, false);
    }

    [ContextMenu("Camera Shake")]
    public void CameraShake()
    {
        MoveCamera(shakeTreeCamPosition, shakeTreeCamRotation, false);
    }
#endif
}
