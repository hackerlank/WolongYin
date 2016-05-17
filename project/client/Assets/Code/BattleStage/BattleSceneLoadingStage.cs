using UnityEngine;
using System.Collections.Generic;

public class BattleSceneLoadingStage : BattleStageBase
{
    public BattleSceneLoadingStage(GameBattle battle)
        : base(battle)
    {

    }


    public override void OnEnter()
    {
        LoadingWindow.Get().Open();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (theBattle.ActiveScene != null)
            LoadingWindow.Get().Progress = 0.5f * theBattle.ActiveScene.Loader.Progress;
    }

    public override void OnExit()
    {
        //LoadingWindow.Get().Destroy();
    }
}