using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class BattleUnit : BaseGameMono, IActionControllerPlayable, IUIEventListener
{
    public enum EState
    {
        idle,
        pre_attack,
        cast_skill,
    }

    private string mGuid = string.Empty;
    private EActionState mActionState = EActionState.stop;
    private EBattleFactionType  mFactionType = EBattleFactionType.FT_Player;
    private CommanderUnit mModel = null;
    private float mPowerNum = 0f;
    private float mHp = 0;
    private StateMechine mUnitStateMechine = new StateMechine();
    private BattleUnitProto mProtoData = null;
    private BattleTile mTile = null;
    private SceneLoader mBindLoader = null;
    private List<BattleSkill> mSkillList = new List<BattleSkill>();
    private bool mDead = false;

    #region Get&Set
    public bool Dead
    {
        get { return mDead; }
        set { mDead = value; }
    }

    public string Guid
    {
        get { return mGuid; }
        private set { mGuid = value; }
    }

    public SceneLoader BindLoader
    {
        get { return mBindLoader; }
        set { mBindLoader = value; }
    }

    public List<BattleSkill> SkillList
    {
        get { return mSkillList; }
    }

    public ProtoBuf.EBattleFactionType FactionType
    {
        get { return mFactionType; }
        private set { mFactionType = value; }
    }

    public BattleTile theTile
    {
        get { return mTile; }
        private set { mTile = value; }
    }

    public BattleUnit.EState ActiveStateType
    {
        get { return (BattleUnit.EState)UnitStateMechine.ActiveStateType; }
    }

    public UnitStateBase ActiveState
    {
        get { return (UnitStateBase) UnitStateMechine.ActiveState; }
    }

    public StateMechine UnitStateMechine
    {
        get { return mUnitStateMechine; }
    }

    public CommanderUnit Model
    {
        get { return mModel; }
        private set { mModel = value; }
    }

    public int PowerStarLevel
    {
        get { return Mathf.FloorToInt(PowerNum / GameSetupXmlClass.instance.battle.one_star_power_val); }
    }

    public float PowerNum
    {
        get { return mPowerNum; }
        private set { mPowerNum = value; }
    }

    public bool MainCommander
    {
        get { return ProtoData.MainCommander; }
    }

    public float Hp
    {
        get { return mHp;  }
        set { mHp = value; }
    }

    public BattleUnitProto ProtoData
    {
        get { return mProtoData; }
        private set { mProtoData = value; }
    }
    #endregion

    #region Mono

    public override void Awake()
    {

    }

    public override void Update(float deltaTime)
    {
        if (Model != null)
            Model.Update(deltaTime);
    }
    #endregion

    #region IActionControllerPlayable
    public EActionState actionState
    {
        get { return mActionState; }
        private set { mActionState = value; }
    }

    public void Pause()
    {
        actionState = EActionState.pause;
        if (Model != null)
            Model.Pause();
    }

    public void Resume()
    {
        actionState = EActionState.playing;
        if (Model != null)
            Model.Resume();
    }

    public void Stop()
    {
        actionState = EActionState.stop;
        if (Model != null)
            Model.Stop();
    }
    #endregion

    #region IUIEventListener
    void IUIEventListener.OnClick(GameObject go)
    {
    }

    void IUIEventListener.OnHover(GameObject go, bool state)
    {
    }

    void IUIEventListener.OnPress(GameObject go, bool press)
    {
    }

    void IUIEventListener.OnDragStart(GameObject go)
    {
    }

    void IUIEventListener.OnDrag(GameObject go, Vector2 delta)
    {
    }

    void IUIEventListener.OnDragEnd(GameObject go)
    {
    }
    #endregion

    #region Battle

    public BattleSkill GetCommandSkill()
    {
        return mSkillList.Find((BattleSkill bs) => { return bs.CommandSkill; });
    }

    public BattleSkill GetNormalSkill()
    {
        return mSkillList.Find((BattleSkill bs) => { return bs.NormalSkill; });
    }

    public BattleSkill FindSkill(int skillID)
    {
        return mSkillList.Find((value) => { return value.Table.baseid == skillID; });
    }

    public void ChangeState(BattleUnit.EState type)
    {
        UnitStateMechine.SetActiveState((int)type);
    }

    public void TryAttack()
    {
        ChangeState(EState.pre_attack);

        BattleSkill sk = mSkillList.Find((value)=> { return PowerNum >= value.PowerRequst; });
        if (sk == null)
            sk = GetNormalSkill();

        bool crit = BattleFormula.CalcCrit(this);

        DoAttackCmd cmd = new DoAttackCmd();
        cmd.Guid = Guid;
        cmd.SkillID = (int)sk.Table.baseid;
        cmd.FactionType = FactionType;
        cmd.RoundCount = GameBattle.instance.RoundCount;
        cmd.IsCrit = crit;
        BattleCmder.instance.SendTryAttackCmd(cmd);
    }

    public void CastSkill(int skillID, bool crit)
    {
        ChangeState(EState.cast_skill);

        BattleSkill sk = FindSkill(skillID);
        if (sk == null)
        {
            Logger.instance.Error("找不到技能: {0}!\n", skillID);
            return;
        }
    }

    public void OnHited(BattleUnit attacker, AttackDefinition attackData)
    {
        
    }
    #endregion

    #region Parse
    private void Parse(BattleUnitProto proto)
    {
        Guid = proto.Guid;
        ProtoData = proto;
        Hp = proto.HP;
        PowerNum = proto.Power;

        if (Model != null)
            Model.Destroy();

        CommanderUnit model = GameUnit.Create<CommanderUnit>("", proto.TableID, BindLoader);
        Model = model;

        Utility.SetIdentityChild(gameObject, Model.gameObject);

        for (int i = 0; i < mSkillList.Count; i++)
        {
            BattleSkill bs = mSkillList[i];
            ObjectPool.Delete<BattleSkill>(bs);
        }
        mSkillList.Clear();

        for (int i = 0; i < ProtoData.SkillList.Count; ++i)
        {
            BattleSkill bs = ObjectPool.New<BattleSkill>();
            bs.OnInit(ProtoData.SkillList[i], this);
            mSkillList.Add(bs);
        }
    }


    public static BattleUnit Create(BattleUnitProto proto, EBattleFactionType factionType, SceneLoader loader)
    {
        GameObject go = new GameObject();
        GameMonoAgent agent = go.AddComponent<GameMonoAgent>();
        BattleUnit btUnit = agent.AddGameMonoComponent<BattleUnit>();
        btUnit.FactionType = factionType;
        btUnit.theTile = factionType == EBattleFactionType.FT_Player
            ? BattleField.instance.PlayerField.GetTile(proto.MainTileIndex)
            : BattleField.instance.EnemyField.GetTile(proto.MainTileIndex);

        btUnit.BindLoader = loader;
        btUnit.Parse(proto);

        return null;
    }
    #endregion
}