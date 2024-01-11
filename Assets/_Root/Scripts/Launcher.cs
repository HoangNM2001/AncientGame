using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pancake;

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
