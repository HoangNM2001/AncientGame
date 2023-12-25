using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEditor.AssetImporters;
using UnityEngine;

public class FishingController : GameComponent
{
    [Header("Moving")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private LeftRightCouple moveLR;

    [Header("Fishing")]
    [SerializeField] private FishingData fishingData;
    [SerializeField] private Trigger hand;
    [SerializeField] private LeftRightCouple handLR;
    [SerializeField] private LineRenderer line;
    [SerializeField] private float downSpeed;
    [SerializeField] private float upSpeed;
    [SerializeField] private float upSpeedCached;
    [SerializeField] private GameObject fx;

    private bool moveToRight;
    private bool isActivated;
    private bool isUp;
    private Fish fishCaught;
    private Coroutine fireCoroutine;

    private bool Firing { get; set; }

    public void Activate()
    {
        Firing = false;
        isActivated = true;
        moveToRight = true;
        isUp = false;
        fishCaught = null;

        transform.localPosition = Vector3.zero;
        SetHandPostion(handLR.leftTrans.localPosition);

        hand.EnterTriggerEvent += OnTriggerEnterEvent;
    }

    public void Deactivate()
    {
        if (fishCaught)
        {
            Destroy(fishCaught.gameObject);
            fishCaught = null;
        }

        hand.EnterTriggerEvent -= OnTriggerEnterEvent;
    }

    protected override void Tick()
    {
        if (!isActivated) return;

        if (!Firing) Move();

        if (!Firing && fishingData.FishCount > 0 && Input.GetMouseButtonDown(0) && !InputUtils.IsPointerOverUI(Input.mousePosition)) Fire();
    }

    private void Fire()
    {
        fishingData.Spend();
        
        Firing = true;
        isUp = false;
        fishCaught = null;

        if (fireCoroutine != null) StopCoroutine(fireCoroutine);

        fireCoroutine = StartCoroutine(IEFire());
    }

    private IEnumerator IEFire()
    {
        while (!fishCaught && !SimpleMath.InRange(hand.transform.localPosition, handLR.rightTrans.transform.localPosition, 0.1f, true))
        {
            SetHandPostion(Vector3.MoveTowards(hand.transform.localPosition, handLR.rightTrans.transform.localPosition, downSpeed * Time.deltaTime));

            yield return null;
        }

        isUp = true;

        while (!SimpleMath.InRange(hand.transform.localPosition, handLR.leftTrans.transform.localPosition, 0.1f, true))
        {
            SetHandPostion(Vector3.MoveTowards(hand.transform.localPosition, handLR.leftTrans.transform.localPosition, (fishCaught ? upSpeedCached : upSpeed) * Time.deltaTime));

            yield return null;
        }

        if (fishCaught)
        {
            Destroy(fishCaught.gameObject);
            fishCaught = null;
        }

        Firing = false;
    }

    private void SetHandPostion(Vector3 pos)
    {
        hand.transform.localPosition = pos;
        line.SetPosition(1, pos);
    }

    private void Move()
    {
        if (moveToRight)
        {
            if (!SimpleMath.InRange(transform.localPosition, moveLR.rightTrans.transform.localPosition, 0.1f))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveLR.rightTrans.transform.localPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                moveToRight = !moveToRight;
            }
        }
        else
        {
            if (!SimpleMath.InRange(transform.localPosition, moveLR.leftTrans.transform.localPosition, 0.1f))
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveLR.leftTrans.transform.localPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                moveToRight = !moveToRight;
            }
        }
    }

    private void OnTriggerEnterEvent(Collider fishCollider)
    {
        if (isUp || fishCaught != null) return;

        if (fishCollider.TryGetComponent<Fish>(out var fish))
        {
            var fxCaught = Instantiate(fx, fishCollider.transform);
            fxCaught.transform.position = fishCollider.transform.position;
            Destroy(fxCaught, 1.5f);
        
            fishCaught = fish;
            fishCaught.OnCaught(hand.transform);
        }
    }
}
