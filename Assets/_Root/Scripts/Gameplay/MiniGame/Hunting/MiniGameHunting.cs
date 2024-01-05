using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class MiniGameHunting : GameComponent, IMiniGame
{
    [SerializeField] private EnumPack.MiniGameType miniGameType;
    [SerializeField] private GameObject container;
    [SerializeField] private HuntingController huntingController;
    [SerializeField] private Transform playerStartPos;
    [SerializeField] private Transform predatorStartPos;
    [SerializeField] private ScriptableEventGetGameObject getCurrentMonsterEvent;
    [SerializeField] private ScriptableEventBool forceStopMinigameEvent;
    [SerializeField] private PredatorVariable predatorVariable;
    [SerializeField] private List<Predator> predatorPrefabList;
    [SerializeField] private Transform cameraTrans;
    [SerializeField] private float camFollowSpeed;

    public EnumPack.MiniGameType MiniGameType => miniGameType;
    public bool IsPlayerTurn { get; private set; }

    private Predator predator;
    private int remainStep;
    private float stepSize;

    public void Activate()
    {
        huntingController.transform.position = playerStartPos.position;
        huntingController.transform.forward = Vector3.right;

        var huntingField = getCurrentMonsterEvent.Raise().GetComponent<HuntingField>();
        predator = Instantiate(predatorPrefabList.FirstOrDefault(p => p.PredatorType == huntingField.Predator.PredatorType),
            container.transform);
        predator.transform.position = predatorStartPos.position;
        predator.transform.forward = Vector3.left;
        predatorVariable.Value = predator;

        ShowNextHit();
        remainStep = predator.MaxStep;
        stepSize = Mathf.Abs(playerStartPos.position.x - predatorStartPos.position.x) / predator.MaxStep;

        container.SetActive(true);
        huntingController.Activate(this);
        predator.Activate();
    }

    public void Deactivate()
    {
        container.SetActive(false);
        huntingController.ClearSpear();

        if (predator != null) Destroy(predator.gameObject);

        DOTween.Kill(this);
    }

    private void ShowNextHit()
    {
        IsPlayerTurn = true;
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() => predator.ShowNextHitPoint()).SetTarget(this);
    }

    public void OnHit()
    {
        if (predator.CurrentHp > 0)
        {
            ShowNextHit();
        }
        else
        {
            EndMiniGame(true);
        }
    }

    public void OnMiss()
    {
        remainStep--;
        predator.MoveForward(1, stepSize, () =>
        {
            if (remainStep <= 0)
            {
                predator.Attack(() =>
                {
                    huntingController.DoDie();
                    EndMiniGame(false);
                });
            }
            else
            {
                ShowNextHit();
            }
        });
    }

    private void EndMiniGame(bool isWin)
    {
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() => forceStopMinigameEvent.Raise(isWin)).SetTarget(this);
    }

    public void OnRelease()
    {
        IsPlayerTurn = false;
    }
}