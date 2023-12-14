using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private ScriptableEventNoParam forceStopMinigameEvent;
    [SerializeField] private PredatorVariable predatorVariable;
    [SerializeField] private List<Predator> predatorPrefabList;

    public EnumPack.MiniGameType MiniGameType => miniGameType;
    public bool IsPlayerTurn { get; private set; }
    public Predator Predator { get; private set; }

    private int remainStep;
    private float stepSize;

    public void Activate()
    {
        huntingController.transform.position = playerStartPos.position;
        huntingController.transform.forward = Vector3.right;

        var mapPredator = getCurrentMonsterEvent.Raise().GetComponent<MapPredator>();
        Predator = Instantiate(predatorPrefabList.FirstOrDefault(p => p.PredatorType == mapPredator.PredatorType), container.transform);
        Predator.transform.position = predatorStartPos.position;
        Predator.transform.forward = Vector3.left;
        predatorVariable.Value = Predator;

        IsPlayerTurn = true;
        remainStep = Predator.MaxStep;
        stepSize = Mathf.Abs(playerStartPos.position.x - predatorStartPos.position.x) / Predator.MaxStep;

        container.SetActive(true);
        huntingController.Activate(this);
        Predator.Activate();
    }

    public void Deactivate()
    {
        container.SetActive(false);
        huntingController.ClearSpear();
    }

    public void OnHit()
    {
        if (Predator.CurrentHp > 0)
        {
            IsPlayerTurn = true;
        }
        else
        {
            
        }
    }

    public void OnMiss()
    {
        remainStep--;
        Predator.MoveForward(1, stepSize, () =>
        {
            if (remainStep <= 0)
            {
                Predator.Attack(() =>
                {
                    huntingController.DoDie();
                    EndMinigame();
                });
            }
            else
            {
                IsPlayerTurn = true;
            }
        });
    }

    private void EndMinigame()
    {
        Debug.LogError("EndMiniGame");
    }

    public void OnRelease()
    {
        IsPlayerTurn = false;
    }
}
