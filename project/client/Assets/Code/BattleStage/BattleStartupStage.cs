using UnityEngine;
using System.Collections.Generic;

public class BattleStartupStage : BattleStageBase
{
    public BattleStartupStage(GameBattle battle)
        : base(battle)
    {

    }


    public override void OnEnter()
    {
        // tmp
        theBattle.ChangeStage(GameBattle.EStage.round_start);
    }

    public override void OnUpdate(float deltaTime)
    {
    }

    public override void OnExit()
    {
    }
}