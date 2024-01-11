using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Pancake;
using UnityEngine.SceneManagement;

public class LoadingScreen : PersistentSingleton<LoadingScreen>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image progress;
    [SerializeField] private float minLoadTime = 1f;

    public bool Loading { get; private set; }

    public void LoadScene(string sceneName, Func<bool> launchCondition = null)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, launchCondition));
    }

    IEnumerator LoadSceneRoutine(string sceneName, Func<bool> launchCondition)
    {
        if (Loading) yield break;
        Loading = true;
        canvas.gameObject.SetActive(true);

        var ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        ao.allowSceneActivation = false;

        var t = 0f;

        while (t < minLoadTime || ao.progress < 0.9f)
        {
            t += Time.unscaledDeltaTime;
            progress.fillAmount = Mathf.Min(t / minLoadTime, ao.progress / 0.9f);

            yield return null;
        }

        if (launchCondition != null)
        {
            yield return new WaitUntil(launchCondition);
        }

        ao.allowSceneActivation = true;
    }

    public void OnLoadingFinished()
    {
        Loading = false;
        canvas.gameObject.SetActive(false);
    }
}
