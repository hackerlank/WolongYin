﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameExtensions
{
    //static Dictionary<GameObject, Dictionary<System.Type, Component>> msSceneObjectComMap = new Dictionary<GameObject, Dictionary<Type, Component>>();
    static Dictionary<GameObject, GameUnit> msUnitMap = new Dictionary<GameObject, GameUnit>();

    public static void ClearData()
    {
        //msSceneObjectComMap.Clear();
        msUnitMap.Clear();
    }

//     public static T GetComponentWithSceneObject<T>(this GameObject go) where T : Component
//     {
//         if (msSceneObjectComMap.ContainsKey(go))
//         {
//             if (msSceneObjectComMap[go].ContainsKey(typeof(T)))
//             {
//                 return (T)msSceneObjectComMap[go][typeof(T)];
//             }
//             else
//             {
//                 T c = go.GetComponent<T>();
//                 if (c != null)
//                 {
//                     msSceneObjectComMap[go].Add(typeof(T), (Component)c);
//                     return c;
//                 }
//                 else
//                 {
//                     return null;
//                 }
//             }
//         }
//         else
//         {
//             T c = go.GetComponent<T>();
//             if (c != null)
//             {
//                 msSceneObjectComMap.Add(go, new Dictionary<Type, Component>());
//                 msSceneObjectComMap[go].Add(typeof(T), (Component)c);
//                 return c;
//             }
//             else
//             {
//                 return null;
//             }
//         }
//     }

    public static T GetGameMonoCommponent<T>(this GameObject go) where T : BaseGameMono
    {
        GameMonoAgent agent = go.GetComponent<GameMonoAgent>();
        if (agent) return agent.GetGameMonoComponent<T>();

        return null;
    }

    public static T AddGameMonoComponent<T>(this GameObject go) where T : BaseGameMono
    {
        GameMonoAgent agent = go.GetComponent<GameMonoAgent>();
        if (agent == null)
            agent = go.AddComponent<GameMonoAgent>();

        return agent.AddGameMonoComponent<T>();
    }

    public static void RegisterGameUnit<T>(this GameObject go, T mono) where T : GameUnit
    {
        if (msUnitMap.ContainsKey(go))
        {
            msUnitMap.Remove(go);
        }

        msUnitMap.Add(go, (GameUnit)mono);
    }

//     public static T GetGameMonoCommponent<T>(this BaseGameMono mono) where T : BaseGameMono
//     {
//         return mono.gameObject.GetGameMonoCommponent<T>();
//     }

    public static GameUnit GetGameUnit(this BaseGameMono mono)
    {
        GameUnit unit = null;
        msUnitMap.TryGetValue(mono.gameObject, out unit);
        return unit;
    }


}