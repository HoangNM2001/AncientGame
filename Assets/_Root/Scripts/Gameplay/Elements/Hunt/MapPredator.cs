using Pancake;
using UnityEngine;

public class MapPredator : GameComponent
{
    [Header("Component")]
    [SerializeField] private EnumPack.PredatorType predatorType;
    [SerializeField] private NavmeshController navmeshController;
    [SerializeField] private CharacterAnimController animController;

    [Header("Reward")]
    [SerializeField] private ResourceConfig meatResource; 
    [SerializeField] private int numberOfMeat; 

    [Header("Move")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float changePathTime;

    public EnumPack.PredatorType PredatorType => predatorType;
    public ResourceConfig MeatResource => meatResource;
    public int NumberOfMeat => numberOfMeat;

    private Vector3 wanderCenter;
    private bool isPlayerInSight;
    private float lastChangeTime;

    public void Activate(Vector3 center)
    {
        wanderCenter = center;
        lastChangeTime = -changePathTime;
        transform.localPosition = Vector3.zero;
    }

    protected override void Tick()
    {
        if (isPlayerInSight)
        {
            navmeshController.Stop();
            animController.UpdateIdle2Run(navmeshController.VelocityRatio, Time.deltaTime);
        }
        else
        {
            animController.UpdateIdle2Run(navmeshController.VelocityRatio, Time.deltaTime);
            Wander();
        }
    }

    private void Wander()
    {
        if (Time.time - lastChangeTime > changePathTime)
        {
            lastChangeTime = Time.time;
            var targetPos = wanderCenter + GetRandomPos(wanderRadius);
            navmeshController.MoveByPosition(targetPos, 0.0f, wanderSpeed, rotateSpeed, 0.1f, Time.deltaTime);
        }
    }

    private Vector3 GetRandomPos(float radius)
    {
        return SimpleMath.RandomVector3(true) * radius;
    }

    public void PlayerInSight(bool status)
    {
        isPlayerInSight = status;
    }
}
