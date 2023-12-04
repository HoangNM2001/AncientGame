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
    [SerializeField] private Transform monsterStartPos;
    [SerializeField] private ScriptableEventGetGameObject getCurrentMonsterEvent;
    [SerializeField] private List<Predator> predatorPrefabList;

    public EnumPack.MiniGameType MiniGameType => miniGameType;
    public bool IsPlayerTurn { get; private set; }
    public Predator Predator { get; private set; }

    private int remainStep;

    public void Activate()
    {
        huntingController.transform.position = playerStartPos.position;
        huntingController.transform.forward = Vector3.right;

        var mapPredator = getCurrentMonsterEvent.Raise().GetComponent<MapPredator>();
        Predator = Instantiate(predatorPrefabList.FirstOrDefault(p => p.PredatorType == mapPredator.PredatorType), container.transform);
        Predator.transform.position = monsterStartPos.position;
        Predator.transform.forward = Vector3.left;

        IsPlayerTurn = true;
        remainStep = Predator.MaxStep;

        container.SetActive(true);
        huntingController.Activate(this);
        Predator.Activate();
    }

    public void Deactivate()
    {
        container.SetActive(false);
    }

    public void OnHit()
    {
        Debug.LogError("Hit");
    }

    public void OnMiss()
    {
        Debug.LogError("Miss");
    }

    public void OnRelease()
    {
        IsPlayerTurn = false;
    }
}
