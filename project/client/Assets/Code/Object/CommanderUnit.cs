using UnityEngine;
using System.Collections.Generic;


public class CommanderUnit : GameUnit
{
    protected override bool _OnCreate(int tableid, SceneLoader loader, params object[] args)
    {
        return true;
    }
}