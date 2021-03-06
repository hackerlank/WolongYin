﻿using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;

public class BattleRoundPlayingStage : BattleStageBase
{
    private int mQueneMask = 0;
    private int mPlayerFindIndex = 0;
    private int mEnemyFindIndex = 0;
    private bool mFindNext = false;

    public bool FindNext
    {
        get { return mFindNext; }
        private set { mFindNext = value; }
    }

    public int EnemyFindIndex
    {
        get { return mEnemyFindIndex; }
        private set { mEnemyFindIndex = value; }
    }

    public int PlayerFindIndex
    {
        get { return mPlayerFindIndex; }
        private set { mPlayerFindIndex = value; }
    }

    public int QueneMask
    {
        get { return mQueneMask; }
        private set { mQueneMask = value; }
    }

    public BattleRoundPlayingStage(GameBattle battle)
        : base(battle)
    {

    }


    public override void OnEnter()
    {
        PlayerFindIndex = EnemyFindIndex = 0;
        QueneMask = theBattle.QueneFlag ? 1 : 2;
        FindNext = true;
    }

    public override void OnUpdate(float deltaTime)
    {
        if (theBattle.ActiveUnitInTurn != null)
        {
            if (theBattle.ActiveUnitInTurn.ActiveStateType == BattleUnit.EState.idle)
            {
                if (theBattle.RoundCount >= theBattle.MaxRound)
                {
                    theBattle.BattleResult = EBattleResultType.BR_Tie;
                    theBattle.ChangeStage(GameBattle.EStage.round_end);
                }
                else if (_CheckAllDie(theBattle.PlayerFaction))
                {
                    theBattle.BattleResult = EBattleResultType.BR_Lose;
                    theBattle.ChangeStage(GameBattle.EStage.round_end);
                }
                else if (_CheckAllDie(theBattle.EnemyFaction))
                {
                    theBattle.BattleResult = EBattleResultType.BR_Win;
                    theBattle.ChangeStage(GameBattle.EStage.round_end);
                }
                else
                {
                    FindNext = true;
                }
            }
        }

        if (FindNext)
        {
            BattleUnit unit = _FindAttackUnit();
            if (unit != null)
            {
                ++theBattle.RoundCount;
                unit.TryAttack();
                theBattle.ActiveUnitInTurn = unit;
            }
            FindNext = false;
        }
    }

    public override void OnExit()
    {
    }


    private BattleUnit _FindAttackUnit()
    {
        BattleUnit ret = null;
        if (QueneMask%2 == 0)
        {
            // player
            if (PlayerFindIndex >= theBattle.PlayerFaction.Units.Count)
            {
                PlayerFindIndex = 0;
            }
            ret = theBattle.PlayerFaction.Units[PlayerFindIndex];
            ++PlayerFindIndex;
        }
        else
        {
            // enemy
            if (EnemyFindIndex >= theBattle.EnemyFaction.Units.Count)
            {
                EnemyFindIndex = 0;
            }
            ret = theBattle.EnemyFaction.Units[EnemyFindIndex];
            ++EnemyFindIndex;
        }

        return ret;
    }

    private bool _CheckAllDie(BattleFaction faction)
    {
        for (int i = 0; i < faction.Units.Count; i++)
        {
            if (!faction.Units[i].Dead)
            {
                return false;
            }
        }
        return true;
    }
}