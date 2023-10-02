using System.Collections;
using System.Collections.Generic;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class ButtonStopAction : MonoBehaviour
{
    [SerializeField] private ScriptableEventNoParam stopActionEvent;
    
    private UIButton stopActionButton;

    private void Awake()
    {
        stopActionButton = GetComponent<UIButton>();
    }

    private void Start()
    {
        stopActionButton.onClick.AddListener(StopAction);
    }

    private void StopAction()
    {
        stopActionEvent.Raise();
    }
}
