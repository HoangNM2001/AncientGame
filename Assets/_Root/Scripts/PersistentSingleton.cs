using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (!IsExist)
            {
                instance = FindObjectOfType<T>();
                if (!IsExist)
                {
                    var obj = new GameObject();
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    public static bool IsExist => instance != null;

    protected virtual void Awake()
    {
        if (!Application.isPlaying) return;

        if (!IsExist)
        {
            instance = this as T;
            DontDestroyOnLoad(transform.gameObject);
            enabled = true;
        }
        else
        {
            if (this != instance) Destroy(gameObject);
        }
    }
}
