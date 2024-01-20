using Pancake.Scriptable;
using UnityEngine;

public class HuntingField : SaveDataElement
{
    [SerializeField] private PlayerControllerVariable player;
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private ResourceConfig meatResource;
    [SerializeField] private MapPredator predator;
    [SerializeField] private float respawnInterval;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

    public MapPredator Predator => predator;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Activate();
    }

    private void Activate()
    {
        predator.gameObject.SetActive(true);
        predator.Activate(transform.position);
    }

    private void Deactivate()
    {
        predator.gameObject.SetActive(false);
    }

    public void OnEndMinigame(bool isWin)
    {
        if (isWin)
        {
            HarvestOnWin();
        }
        else
        {
            player.Value.PunishOnLose();
        }
    }

    public void HarvestOnWin()
    {
        for (var i = 0; i < predator.NumberOfMeat; i++)
        {
            var tempFly = predator.MeatResource.flyModelPool.Request();
            tempFly.transform.SetParent(transform);
            tempFly.transform.localPosition = predator.transform.localPosition;
            tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() =>
            {
                playerLevel.AddExp(meatResource.exp);
                ShowFlyText(transform.position, $"+ {playerLevel.ExpUp} Exp");
                predator.MeatResource.flyModelPool.Return(tempFly);
                flyUIEvent.Raise(new FlyEventData
                {
                    resourceType = predator.MeatResource.resourceType,
                    worldPos = tempFly.transform.position
                });
            });
        }

        predator.DropMeat();
        // Deactivate();
    }

    public void PunishOnLose()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHunter>(out var hunter))
        {
            hunter.TriggerActionHunting(gameObject);
            predator.PlayerInSight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IHunter>(out var hunter))
        {
            hunter.ExitTriggerAction();
            predator.PlayerInSight(false);
        }
    }
}
