using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pancake;
using UnityEngine;

public class Spear : GameComponent
{
    private Vector3 velocity;
    private Vector3 gravity;
    private float damage;
    private bool isStop;
    private bool isLaunched;
    private Action<bool> onStopCallback;

    public void Launch(float force, float damage, Action<bool> onStopCallback)
    {
        this.damage = damage;
        this.onStopCallback = onStopCallback;
        velocity = transform.forward * force;
        gravity = Physics.gravity;
        isStop = false;
        isLaunched = true;
    }

    protected override void Tick()
    {
        if (isStop || !isLaunched) return;

        velocity += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isStop || !isLaunched) return;

        isStop = true;

        var predator = other.GetComponentInParent<Predator>();
        if (predator)
        {
            transform.parent = other.transform;
            predator.Hurt(transform);
            predator.TakeDamage(damage);
            onStopCallback?.Invoke(true);
        }
        else
        {
            onStopCallback?.Invoke(false);
        }
    }
}
