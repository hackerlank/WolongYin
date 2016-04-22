using System;
using UnityEngine;


public class SingletonMonoBehavior<T> : MonoBehaviour where T : class
{
    public static T instance { get; private set; }
    protected bool isExist = false;
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            isExist = true; 
            Destroy(gameObject);
        }
    }
}

