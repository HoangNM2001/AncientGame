using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEngine;
using Random = UnityEngine.Random;

public class FruitTree : SaveDataElement
{
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private ResourceConfig fruitResource;
    [SerializeField] private Animator treeAnimator;
    [SerializeField] private Transform standPosition;
    [SerializeField] private Transform lookAtPosition;
    [SerializeField] private GameObject fruitParent;
    [SerializeField] private float timeGrown;
    [SerializeField] private List<GameObject> fruitList;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

    private List<GameObject> remainFruitList = new();
    private List<GameObject> appearFruitList = new();
    private List<GameObject> droppedFruitList = new();
    private DelayHandle grownFruitHandle;

    public ResourceConfig FruitResource => fruitResource;
    public Transform StandPosition => standPosition;
    public Transform LookAtPosition => lookAtPosition;
    public DelayHandle GrownFruitHandle => grownFruitHandle;

    public int CurrentFruitQuantity
    {
        get => Data.Load($"{uniqueId}_fruitCount", fruitList.Count);
        private set => Data.Save($"{uniqueId}_fruitCount", value);
    }

    public int MaxFruitQuantity => fruitList.Count;

    private void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();

        treeAnimator.speed = 2.0f;

        for (int i = 0; i < fruitList.Count; i++)
        {
            if (i < CurrentFruitQuantity)
            {
                fruitList[i].SetActive(true);
                appearFruitList.Add(fruitList[i]);
            }
            else
            {
                fruitList[i].SetActive(false);
                remainFruitList.Add(fruitList[i]);
            }
        }
    }

    protected override void OnEnabled()
    {
        SpawnFruitInterval();
    }

    protected override void OnDisabled()
    {
        grownFruitHandle?.Pause();
    }

    private void SpawnFruitInterval()
    {
        grownFruitHandle = App.Delay(timeGrown, () =>
        {
            if (!remainFruitList.IsNullOrEmpty())
            {
                FruitAppear(remainFruitList[^1]);
            }
            else
            {
                grownFruitHandle.Pause();
            }
        }, isLooped: true);
    }

    public void Shake()
    {
        treeAnimator.CrossFade(Constant.TREE_SHAKE, 0.1f);

        var numOfDrop = Mathf.Min(Random.Range(1, 6), appearFruitList.Count);
        CurrentFruitQuantity -= numOfDrop;

        for (var i = 0; i < numOfDrop; i++)
        {
            var randomIndex = Random.Range(0, appearFruitList.Count);
            var randomFruit = appearFruitList[randomIndex];
            DropFruit(randomFruit);
            appearFruitList.Remove(randomFruit);
            remainFruitList.Add(randomFruit);
        }
    }

    private void FruitAppear(GameObject fruit)
    {
        var defaultScale = fruit.transform.localScale;
        fruit.SetActive(true);
        fruit.transform.localScale = Vector3.zero;
        fruit.transform.DOScale(defaultScale, 0.5f).SetEase(Ease.OutBack);
        appearFruitList.Add(fruit);
        remainFruitList.Remove(fruit);
        CurrentFruitQuantity++;
    }

    private void DropFruit(GameObject randomFruit)
    {
        randomFruit.SetActive(false);
        var tempDropFruit = fruitResource.flyModelPool.Request();
        tempDropFruit.transform.position = randomFruit.transform.position;
        droppedFruitList.Add(tempDropFruit);
    }

    public void ReturnDroppedModel()
    {
        foreach (var droppedFruit in droppedFruitList)
        {
            var dropPosition = droppedFruit.transform.position;
            fruitResource.flyModelPool.Return(droppedFruit);
            flyUIEvent.Raise(new FlyEventData
            {
                resourceType = fruitResource.resourceType,
                worldPos = dropPosition
            });

            playerLevel.AddExp(fruitResource.exp);
            ShowFlyText(dropPosition, $"+ {playerLevel.ExpUp} Exp");
        }

        fruitResource.resourceQuantity.Value += droppedFruitList.Count;
        droppedFruitList.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterHandleTrigger characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(other);
        if (characterHandleTrigger) characterHandleTrigger.TriggerActionTree(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterHandleTrigger characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(other);
        if (characterHandleTrigger) characterHandleTrigger.ExitTriggerAction();
    }

#if UNITY_EDITOR
    [ContextMenu("Get All Fruits")]
    public void GetAllFruits()
    {
        fruitList = new List<GameObject>();

        for (int i = 0; i < fruitParent.transform.childCount; i++)
        {
            fruitList.Add(fruitParent.transform.GetChild(i).gameObject);
        }
    }
#endif
}