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
    [SerializeField] private List<GameObject> farmToolList;
    [SerializeField] private List<ParticleSystem> particleList;
    [SerializeField] private List<Collider> colliderList;

    private List<Field> _fieldList;
    private EnumPack.CharacterActionType _actionType;
    private bool _isPlayer;
    private int _currentIndex;

    private void Awake()
    {
        _fieldList = new List<Field>();
        _currentIndex = -1;
    }

    public void Initialize(EnumPack.CharacterActionType actionType, bool isPlayer)
    {
        _actionType = actionType;
        _isPlayer = isPlayer;
        Deactivate();
    }

    public void Activate(int index)
    {
        if (_currentIndex == index) return;
        _currentIndex = index;

        var toolIndex = Mathf.Clamp(_currentIndex, 0, farmToolList.Count - 1);
        for (var i = 0; i < farmToolList.Count; i++)
        {
            farmToolList[i].SetActive(i == toolIndex);
        }
        
        var colliderIndex = Mathf.Clamp(_currentIndex, 0, colliderList.Count - 1);
        for (var i = 0; i < colliderList.Count; i++)
        {
            colliderList[i].gameObject.SetActive(i == colliderIndex);
        }

        var particleCount = particleList.Count;
        if (particleCount > 0)
        {
            var particleIndex = Mathf.Clamp(_currentIndex, 0, particleCount - 1);
            for (var i = 0; i < particleCount; i++)
            {
                particleList[i].gameObject.SetActive(i == particleIndex);       
            }
        }
        
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _currentIndex = -1;
        _fieldList.Clear();
        gameObject.SetActive(false);

        foreach (var tool in farmToolList)
        {
            tool.gameObject.SetActive(false);
        }

        foreach (var toolCollider in colliderList)
        {
            toolCollider.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var field = CacheCollider.GetField(other);
        if (field) field.DoFarming(_actionType, _isPlayer);
    }
}