using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshController : GameComponent
{
    private NavMeshAgent navMeshAgent;
    private Quaternion targetRotation;
    private float velocityRatio;

    public float VelocityRatio => velocityRatio;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetRotation = navMeshAgent.transform.rotation;
    }

    protected override void Tick()
    {
        velocityRatio = navMeshAgent.speed > 0 ? navMeshAgent.velocity.magnitude / navMeshAgent.speed : 0;
    }

    public void MoveByDirection(Vector3 direction, float moveSpeed, float rotateSpeed, float deltaTime)
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updatePosition = true;
        navMeshAgent.isStopped = false;
            
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.velocity = direction * moveSpeed;
        if (direction != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(direction);
        }
        navMeshAgent.transform.rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, targetRotation, deltaTime * rotateSpeed);
    }
}
