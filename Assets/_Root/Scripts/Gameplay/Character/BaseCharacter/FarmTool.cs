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
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private List<GameObject> farmToolList;
    [SerializeField] private List<ParticleSystem> particleList;
    [SerializeField] private List<Collider> colliderList;

    private List<Field> fieldList;
    private ParticleSystem currentParticle;
    private Collider currentCollider;
    private EnumPack.CharacterActionType actionType;


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
    }

    public void Deactivate()
    {
        fieldList.Clear();
        gameObject.SetActive(false);
        farmToolModel.gameObject.SetActive(false);
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

        if (currentParticle) currentParticle.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        var field = CacheCollider.GetField(other);
        if (field) field.DoFarming(actionType, isPlayer);
    }
}