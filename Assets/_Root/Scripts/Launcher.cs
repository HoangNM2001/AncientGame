using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pancake;
using UnityEditor;

public class Launcher : GameComponent
{
    private bool launchCondition;

    void Awake()
    {
        Application.targetFrameRate = 60;

        launchCondition = true;
    }

    void Start()
    {
        LoadingScreen.Instance.LoadScene("AncientGameScene", () => launchCondition);
    }
}

#if UNITY_EDITOR
    [InitializeOnLoad]
    internal static class SupportEditorSaveDataInternal
    {
        static SupportEditorSaveDataInternal() { EditorApplication.playModeStateChanged += OnPlayModeStateChanged; }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode) Data.SaveAll();
        }
    }
#endif
