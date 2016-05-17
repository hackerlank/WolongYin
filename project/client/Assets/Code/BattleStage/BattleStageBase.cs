using UnityEngine;
using System.Collections.Generic;


public abstract class BattleStageBase : IStage
{
    private GameBattle mBattle = null;
    public GameBattle theBattle
    {
        get { return mBattle; }
        private set { mBattle = value; }
    }

    public BattleStageBase(GameBattle battle)
    {
        theBattle = battle;
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