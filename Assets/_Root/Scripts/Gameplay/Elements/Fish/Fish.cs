using System;
using Pancake;
using Pancake.SceneFlow;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using Constant = Pancake.SceneFlow.Constant;

public class Fish : GameComponent
{
    [SerializeField] private ResourceConfig resourceConfig;
    [SerializeField] private CharacterAnimController animatorController;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    public LeftRightCouple LeftRightCouple { get; private set; }
    public ResourceConfig ResourceConfig => resourceConfig;

    private MiniGameFishing miniGameFishing;
    private bool isView = false;
    private bool moveToRight;
    private float moveSpeed;
    private Transform hand;
    private Vector3 handOffset;

    public void Activate(MiniGameFishing miniGameFishing, LeftRightCouple leftRightCouple, bool isRestore)
    {
        this.miniGameFishing = miniGameFishing;
        LeftRightCouple = leftRightCouple;
        hand = null;
        moveSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        moveToRight = UnityEngine.Random.value < 0.5f;

        if (isRestore)
        {
            transform.position =
                SimpleMath.RandomBetween(LeftRightCouple.leftTrans.position, LeftRightCouple.rightTrans.position);
        }
        else
        {
            transform.position = moveToRight ? LeftRightCouple.leftTrans.position : LeftRightCouple.rightTrans.position;
        }

        UpdateRotation();
        animatorController.Play(Constant.FISH_IDLE, 0);
    }

    protected override void Tick()
    {
        if (isView) return;

        if (hand != null)
        {
            transform.position = hand.position + handOffset;
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        if (moveToRight)
        {
            if (!SimpleMath.InRange(transform.position, LeftRightCouple.rightTrans.position, 0.1f))
            {
                transform.position = Vector3.MoveTowards(transform.position, LeftRightCouple.rightTrans.position,
                    moveSpeed * Time.deltaTime);
            }
            else
            {
                moveToRight = !moveToRight;
            }
        }
        else
        {
            if (!SimpleMath.InRange(transform.position, LeftRightCouple.leftTrans.position, 0.1f))
            {
                transform.position = Vector3.MoveTowards(transform.position, LeftRightCouple.leftTrans.position,
                    moveSpeed * Time.deltaTime);
            }
            else
            {
                moveToRight = !moveToRight;
            }
        }

        UpdateRotation();
    }

    private void UpdateRotation()
    {
        transform.rotation = moveToRight
            ? Quaternion.LookRotation(LeftRightCouple.rightTrans.position - LeftRightCouple.leftTrans.position)
            : Quaternion.LookRotation(LeftRightCouple.leftTrans.position - LeftRightCouple.rightTrans.position);
    }

    public void OnCaught(Transform handTrans)
    {
        hand = handTrans;
        handOffset = transform.position - handTrans.position;
        miniGameFishing.OnFishCaught(this);
        animatorController.Play(Constant.FISH_CAUGHT, 0);
    }

    public void SetView()
    {
        isView = true;
        transform.localScale = Vector3.one * 0.5f;
        MoveAround();
    }

    public void StopMoveAround()
    {
        CancelInvoke(nameof(MoveToPoint));
    }

    public void MoveAround()
    {
        var time = UnityEngine.Random.Range(4.0f, 8.0f);
        var speed = UnityEngine.Random.Range(1.0f, 1.5f);
        navMeshAgent.speed = speed;
        InvokeRepeating(nameof(MoveToPoint), 0.0f, time);
    }

    public void MoveToPoint()
    {
        navMeshAgent.SetDestination(RandomNavmeshLocation(3.0f));
    }

    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
}