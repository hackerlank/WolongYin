using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameExtensions
{
    static Dictionary<GameObject, Dictionary<System.Type, Component>> msSceneObjectComMap = new Dictionary<GameObject, Dictionary<Type, Component>>();

    public static void ClearData()
    {
        msSceneObjectComMap.Clear();
    }

    public static T GetComponentWithSceneObject<T>(this GameObject go) where T : Component
    {
        if (msSceneObjectComMap.ContainsKey(go))
        {
            if (msSceneObjectComMap[go].ContainsKey(typeof(T)))
            {
                return (T)msSceneObjectComMap[go][typeof(T)];
            }
            else
            {
                T c = go.GetComponent<T>();
                if (c != null)
                {
                    msSceneObjectComMap[go].Add(typeof(T), (Component)c);
                    return c;
                }
                else
                {
                    return null;
                }
            }
        }
        else
        {
            T c = go.GetComponent<T>();
            if (c != null)
            {
                msSceneObjectComMap.Add(go, new Dictionary<Type, Component>());
                msSceneObjectComMap[go].Add(typeof(T), (Component)c);
                return c;
            }
            else
            {
                return null;
            }
        }
    }

    public static T GetGameMonoCommponent<T>(this GameObject go) where T : BaseGameMono
    {
        return null;
    }

//     public static T GetGameMonoCommponent<T>(this BaseGameMono mono) where T : BaseGameMono
//     {
//         return mono.gameObject.GetGameMonoCommponent<T>();
//     }

    public static GameUnit GetGameUnit(this BaseGameMono mono)
    {
        return GetGameMonoCommponent<GameUnit>(mono.gameObject);
    }


}