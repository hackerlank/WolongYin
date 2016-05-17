using UnityEngine;
using System.Collections.Generic;

public class BattleLoadingStage : BattleStageBase
{
    public BattleLoadingStage(GameBattle battle)
        : base(battle)
    {
        
    }


    public override void OnEnter()
    {
        //LoadingWindow.Get().Open();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (theBattle.BattleLoader != null)
        {
            theBattle.BattleLoader.Update();
            LoadingWindow.Get().Progress = 0.5f + 0.5f*theBattle.BattleLoader.Progress;
        }
    }

    public override void OnExit()
    {
        LoadingWindow.Get().Destroy();
    }
}