using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Pancake;
using UnityEditor.Experimental.GraphView;
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
        velocityRatio = navMeshAgent.speed > 0.1f ? navMeshAgent.velocity.magnitude / navMeshAgent.speed : 0;
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

        navMeshAgent.transform.rotation =
            Quaternion.Slerp(navMeshAgent.transform.rotation, targetRotation, deltaTime * rotateSpeed);
    }

    public void MoveByPosition(Vector3 targetPosition, float targetRadius, float moveSpeed, float rotateSpeed,
        float stoppingDistance, float deltaTime)
    {
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = stoppingDistance;

        var direction = targetPosition - navMeshAgent.transform.position;
        var distance = direction.magnitude - targetRadius - stoppingDistance;

        if (distance <= 0.0f)
        {
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.isStopped = true;
            if (direction != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(direction);
            }

            navMeshAgent.transform.rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, targetRotation,
                deltaTime * rotateSpeed);
        }
        else
        {
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = navMeshAgent.transform.position + direction.normalized * distance;
        }
    }

    public bool IsReachDestination()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f;
    }

    public void ResetPath()
    {
        navMeshAgent.ResetPath();
    }

    public void Stop()
    {
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;
    }

    public void Disable()
    {
        navMeshAgent.enabled = false;
    }

    public void Enable()
    {
        navMeshAgent.enabled = true;
    }
}