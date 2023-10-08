using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using Pancake.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ResourceQuantity : GameComponent
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private Sequence scaleSequence;
    private const float ScaleDuration = 0.12f;

    public EnumPack.ResourceType ResourceType { get; private set; }
    public IntVariable QuantityVariable { get; private set; }
    
    public Vector3 IconPosition => resourceIcon.transform.position; 

    public void Initialize(Sprite icon, EnumPack.ResourceType type, IntVariable variable)
    {
        resourceIcon.sprite = icon;
        ResourceType = type;
        QuantityVariable = variable;
        resourceIcon.SetNativeSize();
        quantityText.SetText(QuantityVariable.Value.ToString());
    }

    public void UpdateValue()
    {
        QuantityVariable.Value++;
        quantityText.SetText(QuantityVariable.Value.ToString());
        ScaleEffect();
    }

    public void UpdateCoinValue(int changeValue)
    {
        // QuantityVariable.Value += changeValue;
        // quantityText.SetText(QuantityVariable.Value.ToString());
        ScaleEffect();
    }

    private void ScaleEffect()
    {
        scaleSequence?.Kill();

        scaleSequence = DOTween.Sequence();

        scaleSequence.Append(resourceIcon.transform.DOScale(1.5f, ScaleDuration));
        scaleSequence.Append(resourceIcon.transform.DOScale(1.0f, ScaleDuration));

        scaleSequence.Play();
    }
}