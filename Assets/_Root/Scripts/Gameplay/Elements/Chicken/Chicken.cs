using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pancake;

public class Chicken : GameComponent
{
    [SerializeField] private NavmeshController navmeshController;
    [SerializeField] private CharacterAnimController animController;

    [Header("Movement")] [SerializeField] private float rotateSpeed;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float changePathTime;

    [Header("SpawnEggs")] [SerializeField] private float spawnEggsInterval;

    private Henhouse _henhouse;
    private float lastTimeSpawn;

    public void Setup(Henhouse henhouse)
    {
        _henhouse = henhouse;
    }

    protected override void Tick()
    {
        animController.UpdateIdle2Run(navmeshController.VelocityRatio, Time.deltaTime);
    }
}