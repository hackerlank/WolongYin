using UnityEngine;
using ProtoBuf;
using System.Collections;


public class BattleCmder : Singleton<BattleCmder>
{
    public void OnBattleStart(StartBattleCmdReceive cmd)
    {
        GameBattle.instance.OnStartBattle(cmd);
    }


    public void SendTryAttackCmd(DoAttackCmd cmd)
    {
        // to do.

        OnTryAttack(cmd);
    }


    public void OnTryAttack(DoAttackCmd cmd)
    {
        BattleUnit unit = null;
        if (cmd.FactionType == EBattleFactionType.FT_Enemy)
        {
           unit = GameBattle.instance.EnemyFaction.Find(cmd.Guid);
        }
        else
        {
            unit = GameBattle.instance.PlayerFaction.Find(cmd.Guid);
        }

        if (unit == null)
        {
            Logger.instance.Error("错误的角色GUID ：{0} !\n", cmd.Guid);
            return;
        }

        unit.CastSkill(cmd.SkillID, cmd.IsCrit);
        GameBattle.instance.ActiveUnitInTurn = unit;
    }

    
    public void TestBattle()
    {
        // to do.
        StartBattleCmdReceive cmd = new StartBattleCmdReceive();

        BattleUnitProto p1 = new BattleUnitProto();

        OnBattleStart(cmd);
    }
}