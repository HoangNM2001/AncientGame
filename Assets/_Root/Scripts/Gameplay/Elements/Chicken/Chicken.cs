using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pancake;

public class Chicken : GameComponent
{
    [SerializeField] private NavmeshController navmeshController;
    [SerializeField] private CharacterAnimController animController;

    [Header("Movement")][SerializeField] private float rotateSpeed;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float changePathTime;

    [Header("SpawnEggs")][SerializeField] private float spawnEggsInterval;

    private Henhouse _henhouse;
    private float lastTimeSpawn;
    private float lastChangeTime;
    private Vector3 wanderCenter;
    private bool isFocus;

    public void Setup(Henhouse henhouse)
    {
        _henhouse = henhouse;
        lastChangeTime = -changePathTime;
        wanderCenter = _henhouse.transform.position;
        isFocus = true;
    }

    protected override void Tick()
    {
        animController.UpdateIdle2Run(navmeshController.VelocityRatio, Time.deltaTime);

        Wander();

        if (_henhouse.CanSpawnEggs)
        {
            if (Time.time - lastTimeSpawn > spawnEggsInterval)
            {
                lastTimeSpawn = Time.time;

                _henhouse.SpawnEgg(transform.position);
            }
        }
    }

    private void Wander()
    {
        if (!isFocus)
        {
            navmeshController.Stop();
            return;
        }

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

    private void OnApplicationFocus(bool focusStatus)
    {
        isFocus = focusStatus;
    }
}