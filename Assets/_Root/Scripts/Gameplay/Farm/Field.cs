using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class Field : GameComponent
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color soilColor;
    [SerializeField] private Color seedColor;
    [SerializeField] private Color drillColor;

    private MaterialPropertyBlock fieldMaterialBlock;

    private void Awake()
    {
        fieldMaterialBlock = new MaterialPropertyBlock();
        
        fieldMaterialBlock.SetColor("_Color", drillColor);
        meshRenderer.SetPropertyBlock(fieldMaterialBlock);
    }
}
