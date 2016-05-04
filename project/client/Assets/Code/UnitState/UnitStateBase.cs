using UnityEngine;
using System.Collections.Generic;


public abstract class UnitStateBase : IStage
{
    protected BattleUnit mUnit = null;

    public UnitStateBase(BattleUnit unit)
    {
        mUnit = unit;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnUpdate(float deltaTime)
    {
    }

    public void OnGUI()
    {
    }
}