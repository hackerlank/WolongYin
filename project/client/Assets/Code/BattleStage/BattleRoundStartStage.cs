using UnityEngine;
using System.Collections.Generic;

public class BattleRoundStartStage : BattleStageBase
{
    public BattleRoundStartStage(GameBattle battle)
        : base(battle)
    {

    }


    public override void OnEnter()
    {
        // tmp
        theBattle.ChangeStage(GameBattle.EStage.round_playing);
    }

    public override void OnUpdate(float deltaTime)
    {
    }

    public override void OnExit()
    {
    }
}