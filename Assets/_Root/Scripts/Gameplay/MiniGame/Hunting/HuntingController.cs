using System.Collections;
using System.Collections.Generic;
using System.Net;
using Pancake;
using Pancake.SceneFlow;
using UnityEngine;

public class HuntingController : GameComponent
{
    [SerializeField] private Camera miniGameCamera;
    [SerializeField] private CharacterAnimController animController;
    [SerializeField] private OrbitDrawer orbitDrawer;
    [SerializeField] private Spear spear;
    [SerializeField] private Transform launchPos;
    [SerializeField] private float launchForce;
    [SerializeField] private float damage;

    private Vector3 launchDirection;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private bool isDragging;
    private int remainStep;
    private MiniGameHunting miniGameHunting;
    private List<Spear> spearList;

    private void Awake()
    {
        spearList = new List<Spear>();
    }

    protected override void OnEnabled()
    {
        animController.OnAnimationEvent += animController_OnAnimationEvent;
    }

    protected override void OnDisabled()
    {
        animController.OnAnimationEvent -= animController_OnAnimationEvent;
    }

    public void Activate(MiniGameHunting miniGame)
    {
        miniGameHunting = miniGame;
        spear.gameObject.SetActive(true);
        animController.Play(Constant.IDLE, 0);

        isDragging = false;
    }

    protected override void Tick()
    {
        if (!miniGameHunting.IsPlayerTurn) return;

        if (Input.GetMouseButtonDown(0)) OnDragStart();

        if (Input.GetMouseButtonUp(0)) OnDragEnd();

        if (isDragging) OnDrag();
    }

    private void OnDragStart()
    {
        isDragging = true;
        orbitDrawer.gameObject.SetActive(true);
        startPoint = miniGameCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnDragEnd()
    {
        isDragging = false;
        orbitDrawer.gameObject.SetActive(false);
        if (launchDirection.x != 0.0f) Release();
    }

    private void OnDrag()
    {
        endPoint = miniGameCamera.ScreenToWorldPoint(Input.mousePosition);
        launchDirection = startPoint - endPoint;
        launchDirection.z = 0.0f;
        orbitDrawer.DrawOrbit(launchDirection);
    }

    private void Release()
    {
        animController.Play(Constant.HUNTING_ATTACK, 0);
        miniGameHunting.OnRelease();
    }

    private void animController_OnAnimationEvent()
    {
        var cloneSpear = Instantiate(spear);
        cloneSpear.transform.position = launchPos.transform.position;
        cloneSpear.transform.rotation = Quaternion.LookRotation(launchDirection);
        cloneSpear.Launch(launchForce * launchDirection.magnitude, damage, OnSpearStop);
        spearList.Add(cloneSpear);
        spear.gameObject.SetActive(false);
    }

    private void OnSpearStop(bool isHit)
    {
        if (isHit)
        {
            miniGameHunting.OnHit();
        }
        else
        {
            miniGameHunting.OnMiss();
        }
        spear.gameObject.SetActive(true);
    }

    public void DoDie()
    {
        animController.Play(Constant.HUNTING_DIE, 0);
    }
}
