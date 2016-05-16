using UnityEngine;
using System.Collections.Generic;


public abstract class BattleStageBase : IStage
{

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

    public void Pause()
    {
    }

    public void Resume()
    {
    }

    public void Stop()
    {
    }
}