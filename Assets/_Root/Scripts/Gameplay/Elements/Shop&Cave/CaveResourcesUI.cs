using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaveResourcesUI : MonoBehaviour
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI fillText;

    private int _maxCapacity;

    public void Setup(Sprite resourceSprite, int currentCapacity, int maxCapacity)
    {
        resourceIcon.sprite = resourceSprite;
        resourceIcon.SetNativeSize();

        _maxCapacity = maxCapacity;
        
        UpdateCapacity(currentCapacity);
    }

    public void UpdateCapacity(int currentCapacity)
    {
        fillBar.fillAmount = (float)currentCapacity / (float)_maxCapacity;
        fillText.SetText($"{currentCapacity}/{_maxCapacity}"); 
    }
}
