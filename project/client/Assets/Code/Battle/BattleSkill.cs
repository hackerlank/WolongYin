using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class BattleSkill : IPoolable
{
    private SkillTable mTable = null;
    private BattleUnit mOwner = null;
    private List<AttackDefinition> mAtkDefList = new List<AttackDefinition>();
    private Utility.VoidDelegate mEndCallback = null;
    private bool mCasting = false;
    private float mCurTime = 0f;
    private int mAtkDefIndex = 0;
    private bool mIsCrit = false;
    private List<BattleUnit> mHitedUnits = new List<BattleUnit>();

    #region Get&Set
    public List<BattleUnit> HitedUnits
    {
        get { return mHitedUnits; }
        private set { mHitedUnits = value; }
    }

    public GameDef.ESkillTargetType TargetType
    {
        get { return (GameDef.ESkillTargetType) Table.targetType; }
    }

    public GameDef.ESkillHitType HitType
    {
        get { return (GameDef.ESkillHitType) Table.hitType; }
    }

    public List<AttackDefinition> AtkDefList
    {
        get { return mAtkDefList; }
        private set { mAtkDefList = value; }
    }

    public float CurTime
    {
        get { return mCurTime; }
        private set { mCurTime = value; }
    }

    public bool Casting
    {
        get { return mCasting; }
        private set { mCasting = value; }
    }

    public Utility.VoidDelegate CastEndCallback
    {
        get { return mEndCallback; }
        private set { mEndCallback = value; }
    }

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

    public bool CommandSkill
    {
        get { return Table.type == (int)GameDef.ESkillType.command; }        
    }

    public bool SpSkill
    {
        get { return Table.type == (int)GameDef.ESkillType.sp; }
    }

    public int PowerRequst
    {
        get { return Table.powerRequst; }
    }

    #endregion

    public void OnInit(int id, BattleUnit owner)
    {
        mOwner = null;
        mTable = SkillTableManager.instance.Find((uint)id);
        CurTime = 0f;
        Casting = false;
    }

    public void CreateAttackDefinition(AttackDefProto proto)
    {
        AttackDefinition atk = ObjectPool.New<AttackDefinition>();
        atk.ProtoData = proto;
        atk.Owner = atk.RealOwner = Owner;

        atk.OnStart();

        AtkDefList.Add(atk);
    }

    public void OnUpdate(float deltaTime)
    {
        if (!Casting)
            return;

        CurTime += deltaTime;
        _ProcessAttackDefinition(CurTime);
        _TickSkill(deltaTime);
    }

    public void Cast(bool crit, Utility.VoidDelegate callback)
    {
        _ClearAtkDef();
        CastEndCallback = callback;
        Casting = true;
        CurTime = 0f;
        mIsCrit = crit;
        mAtkDefIndex = 0;
        _ListTargets();
    }


    void _ListTargets()
    {
        HitedUnits.Clear();
        if (TargetType == GameDef.ESkillTargetType.friend)
        {
            _ListTargets(GameBattle.instance.PlayerFaction);
        }
        else if (TargetType == GameDef.ESkillTargetType.enemy)
        {
            _ListTargets(GameBattle.instance.EnemyFaction);
        }
    }

    //630 036
    //741 147
    //852 258

    void _ListTargets(BattleFaction faction)
    {
        switch (HitType)
        {
            case GameDef.ESkillHitType.single:
            {
                BattleUnit first = null;
                BattleTile bt = faction.theField.GetTile(Owner.theTile.Row, Owner.theTile.Column);
                if (bt.TheUnit != null
                    && _CheckTarget(bt.TheUnit))
                {
                    first = bt.TheUnit;
                }

                if (first != null)
                {
                    for (int i = 0; i < faction.Units.Count; i++)
                    {
                        BattleUnit unit = faction.Units[i];
                        if (!_CheckTarget(unit))
                            continue;

                        HitedUnits.Add(unit);
                        break;
                    }
                }
                break;
            }
            case GameDef.ESkillHitType.back_single:
            {
                BattleUnit first = null;
                BattleTile bt = faction.theField.GetTile(Owner.theTile.Row, GameBattle.instance.ActiveScene.theField.Cols - 1);
                if (bt.TheUnit != null
                    && _CheckTarget(bt.TheUnit))
                {
                    first = bt.TheUnit;
                }

                if (first == null)
                {
                    for (int i = faction.Units.Count - 1; i >= 0; i--)
                    {
                        BattleUnit unit = faction.Units[i];
                        if (!_CheckTarget(unit))
                            continue;

                        HitedUnits.Add(unit);
                        break;
                    }
                }
                break;
            }
            case GameDef.ESkillHitType.random_single:
            {
                while (true)
                {
                    int idx = Random.Range(0, faction.Units.Count);
                    BattleUnit unit = faction.Units[idx];
                    if (!_CheckTarget(unit))
                        continue;

                    HitedUnits.Add(unit);
                    break;
                }
                break;
            }
            case GameDef.ESkillHitType.row:
            {
                BattleUnit first = null;
                for (int i = 0; i < faction.Units.Count; i++)
                {
                    BattleUnit unit = faction.Units[i];
                    if (!_CheckTarget(unit))
                        continue;

                    first = unit;
                    break;
                }

                if (first != null)
                {
                    HitedUnits.Add(first);

                    int idx = first.theTile.Index;
                    int mx = GameBattle.instance.ActiveScene.theField.Rows*GameBattle.instance.ActiveScene.theField.Cols -
                             1;
                    while (idx <= mx)
                    {
                        idx += GameBattle.instance.ActiveScene.theField.Cols;

                        BattleUnit unit = faction.Units[idx];
                        if (!_CheckTarget(unit))
                            continue;

                        HitedUnits.Add(unit);
                    }
                }
                break;
            }
            case GameDef.ESkillHitType.column:
            {
                BattleUnit first = null;
                for (int i = 0; i < faction.Units.Count; i++)
                {
                    BattleUnit unit = faction.Units[i];
                    if (!_CheckTarget(unit))
                        continue;

                    first = unit;
                    break;
                }

                if (first != null)
                {
                    HitedUnits.Add(first);

                    for (int i = 0; i < faction.Units.Count; i++)
                    {
                        BattleUnit unit = faction.Units[i];
                        if (!_CheckTarget(unit))
                            continue;

                        if (first.theTile.Column != unit.theTile.Column)
                            continue;
                        
                        HitedUnits.Add(unit);
                    }
                }
                break;
            }
            case GameDef.ESkillHitType.back_column:
            {
                BattleUnit first = null;
                for (int i = faction.Units.Count - 1; i >= 0; i--)
                {
                    BattleUnit unit = faction.Units[i];
                    if (!_CheckTarget(unit))
                        continue;

                    first = unit;
                    break;
                }

                if (first != null)
                {
                    HitedUnits.Add(first);

                    for (int i = 0; i < faction.Units.Count; i++)
                    {
                        BattleUnit unit = faction.Units[i];
                        if (!_CheckTarget(unit))
                            continue;

                        if (first.theTile.Column != unit.theTile.Column)
                            continue;

                        HitedUnits.Add(unit);
                    }
                }
                break;
            }
            case GameDef.ESkillHitType.all:
            {
                for (int i = 0; i < faction.Units.Count; i++)
                {
                    BattleUnit unit = faction.Units[i];
                    if (!_CheckTarget(unit))
                        continue;

                    HitedUnits.Add(unit);
                }
                break;
            }
            case GameDef.ESkillHitType.random_all:
            {
                int c = Table.hitCount > faction.Units.Count ? faction.Units.Count : Table.hitCount;
                int rc = 0;
                int mask = 0;

                for (int i = 0; i < faction.Units.Count; i++)
                {
                    BattleUnit unit = faction.Units[i];
                    if (!_CheckTarget(unit))
                        continue;

                    HitedUnits.Add(unit);
                    ++rc;
                }

                while (c < rc)
                {
                    int idx = Random.Range(0, HitedUnits.Count);

                    HitedUnits.RemoveAt(idx);
                    --rc;
                }
                break;
            }
        }
    }

    bool _CheckTarget(BattleUnit unit)
    {
        if (unit == null)
            return false;

        if (unit.Dead)
            return false;

        // to do.

        return true;
    }

    void _ProcessAttackDefinition(float curTime)
    {
        UnitActionProto proto = UnitActionHelper.FindProto(Owner.ProtoData.TableID);
        while (mAtkDefIndex < proto.atkDefList.Count)
        {
            AttackDefProto atk = proto.atkDefList[mAtkDefIndex];
            if (atk.triggerTime > curTime)
                break;

            CreateAttackDefinition(atk);
            mAtkDefIndex++;
        }
    }

    void _TickSkill(float deltaTime)
    {
        bool end = true;
        for (int i = 0; i < AtkDefList.Count; ++i)
        {
            AttackDefinition adf = AtkDefList[i];
            adf.OnUpdate(deltaTime);

            if (adf.OutOfData)
            {
                ObjectPool.Delete<AttackDefinition>(adf);
                AtkDefList.RemoveAt(i);
                --i;
            }
            else
            {
                end = false;
            }
        }

        if (end)
        {
            Casting = false;
            if (CastEndCallback != null)
                CastEndCallback();
            CastEndCallback = null;
        }
    }

    void _Reset()
    {
        mTable = null;
        mOwner = null;
        _ClearAtkDef();
        Casting = false;
        CastEndCallback = null;
        CurTime = 0f;
        mIsCrit = false;
        mAtkDefIndex = 0;
        HitedUnits.Clear();
    }

    void _ClearAtkDef()
    {
        for (int i = 0; i < AtkDefList.Count; ++i)
        {
            AttackDefinition adf = AtkDefList[i];
            ObjectPool.Delete<AttackDefinition>(adf);
        }
        AtkDefList.Clear();
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