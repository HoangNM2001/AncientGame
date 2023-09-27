using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Pancake;
using UnityEditor;
using UnityEngine;

public class FarmTool : GameComponent
{
    [SerializeField] private GameObject farmToolModel;
    [SerializeField] private List<ParticleSystem> particleList;
    [SerializeField] private List<Collider> colliderList;
    [SerializeField] private float customUpdateDelay = 0.25f;

    private List<Field> fieldList;
    private ParticleSystem currentParticle;
    private Collider currentCollider;
    private EnumPack.CharacterActionType actionType;
    private DelayHandle delayHandle;


    private void Awake()
    {
        fieldList = new List<Field>();
    }

    public void Initialize(EnumPack.CharacterActionType actionType)
    {
        this.actionType = actionType;
        Deactivate();
    }

    public void Activate(int currentLevel)
    {
        gameObject.SetActive(true);
        farmToolModel.gameObject.SetActive(true);
        UpdateTool(currentLevel);
        MyUpdate();
    }

    public void Deactivate()
    {
        fieldList.Clear();
        gameObject.SetActive(false);
        farmToolModel.gameObject.SetActive(false);
        if (delayHandle != null) App.CancelDelay(delayHandle);
    }

    private void MyUpdate()
    {
        delayHandle = App.Delay(customUpdateDelay, () =>
        {
            foreach (var field in fieldList)
            {
                field.DoFarming(actionType);
            }
        }, isLooped: true);
    }

    private void UpdateTool(int currentLevel)
    {
        if (currentParticle) currentParticle.gameObject.SetActive(false);
        if (currentCollider) currentCollider.gameObject.SetActive(false);

        if (particleList.Count > 0)
        {
            currentParticle = particleList[currentLevel - 1];
            currentParticle.gameObject.SetActive(true);
        }

        if (colliderList.Count > 0)
        {
            currentCollider = colliderList[currentLevel - 1];
            currentCollider.gameObject.SetActive(true);
        }
    }

    public void PlayFarmParticle()
    {
        currentParticle.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        var tempField = CachedCollider.GetField(other);
        if (tempField) fieldList.Add(tempField);
    }

    private void OnTriggerExit(Collider other)
    {
        var tempField = CachedCollider.GetField(other);
        if (tempField) fieldList.Remove(tempField);
    }
}