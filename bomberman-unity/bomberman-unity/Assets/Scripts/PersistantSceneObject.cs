using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistantSceneObject<T> : MonoBehaviour where T : class
{
    private static T instance;

    public static T GetInstanceOrNull()
    {
        return instance;
    }

    public static T GetInstance()
    {
        Debug.Assert(instance != null);
        return instance;
    }


    public static bool Destroyed()
    {
        return instance == null;
    }

    protected virtual void Awake()
    {
        Debug.Assert(instance == null);
        instance = this as T;
        Debug.Assert(instance != null);
    }

    protected virtual void OnDestroy()
    {
        if (instance != null)
        {
            instance = null;
        }
    }
}
