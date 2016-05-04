using UnityEngine;
using System.Collections.Generic;


public class BattleSkill : IPoolable
{
    private SkillTable mTable = null;
    private BattleUnit mOwner = null;

    public BattleUnit Owner
    {
        get { return mOwner; }
    }

    public SkillTable Table
    {
        get { return mTable; }
    }

    public bool NormalSkill
    {
        get { return Table.type == (int)GameDef.ESkillType.normal; }
    }

    public int PowerRequst
    {
        get { return Table.powerRequst; }
    }

    public BattleSkill(int id, BattleUnit owner)
    {
        mOwner = null;
        mTable = SkillTableManager.instance.Find((uint)id);
    }

    public void OnUpdate(float deltaTime)
    {
        
    }

    void _Reset()
    {
        mTable = null;
        mOwner = null;
    }

    void IPoolable.Create()
    {
    }

    void IPoolable.New()
    {
    }

    void IPoolable.Delete()
    {
        _Reset();
    }
}