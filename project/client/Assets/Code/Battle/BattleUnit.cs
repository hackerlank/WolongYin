using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class BattleUnit : BaseGameMono, IActionControllerPlayable, IUIEventListener
{
    private string mGuid = string.Empty;
    private EActionState mActionState = EActionState.stop;
    private EBattleFactionType  mFactionType = EBattleFactionType.FT_Player;
    private CommanderUnit mModel = null;
    private float mPowerNum = 0f;
    private float mHp = 0;
    private StateMechine mUnitStateMechine = new StateMechine();
    private BattleUnitProto mProtoData = null;
    private BattleTile mTile = null;

    #region Get&Set
    public string Guid
    {
        get { return mGuid; }
        private set { mGuid = value; }
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
        get { return Mathf.FloorToInt(mPowerNum/GameSetupXmlClass.instance.battle.one_star_power_val); }
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

    public void ChangeState(GameDef.EUnitState type)
    {
        UnitStateMechine.SetActiveState((int)type);
    }

    public void TryAttack()
    {
        
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

        CommanderUnit model = GameUnit.Create<CommanderUnit>("", proto.TableID, null);
        Model = model;

        Utility.SetIdentityChild(gameObject, Model.gameObject);
    }


    public static BattleUnit Create(BattleUnitProto proto, EBattleFactionType factionType)
    {
        GameObject go = new GameObject();
        GameMonoAgent agent = go.AddComponent<GameMonoAgent>();
        BattleUnit btUnit = agent.AddGameMonoComponent<BattleUnit>();
        btUnit.FactionType = factionType;
        btUnit.theTile = factionType == EBattleFactionType.FT_Player
            ? BattleField.instance.PlayerField.GetTile(proto.MainTileIndex)
            : BattleField.instance.EnemyField.GetTile(proto.MainTileIndex);

        btUnit.Parse(proto);

        return null;
    }
    #endregion
}