using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;

public class GameBattle : Singleton<GameBattle>, IActionControllerPlayable
{
    public enum EStage
    {
        scene_loading,
        battle_loading,
        start,
        round_start,
        round_playing,
        round_end,
        end,
    }

    private int mRoundCount = 0;
    private int mMaxRound = 1;
    private EActionState mActionState = EActionState.stop;
    private StateMechine mBattleStageMechine = new StateMechine();
    private BattleFaction mPlayerFaction = null;
    private BattleFaction mEnemyFaction = null;
    private BattleScene mActiveScene = null;
    private SceneLoader mBattleLoader = null;
    private bool mQueneFlag = false; // false = player, true = enemy
    private BattleUnit mActiveUnitInTurn = null;
    private EBattleResultType mResult = EBattleResultType.BR_Tie;
    private AttackDefMapProto mAtkDefMapProto = null;


    #region Get&Set
    public AttackDefMapProto AtkDefMapProto
    {
        get { return mAtkDefMapProto; }
        private set { mAtkDefMapProto = value; }
    }

    public EBattleResultType BattleResult
    {
        get { return mResult; }
        set { mResult = value; }
    }

    public bool QueneFlag
    {
        get { return mQueneFlag; }
        set { mQueneFlag = value; }
    }

    public BattleUnit ActiveUnitInTurn
    {
        get { return mActiveUnitInTurn; }
        set { mActiveUnitInTurn = value; }
    }

    public SceneLoader BattleLoader
    {
        get { return mBattleLoader; }
        private set { mBattleLoader = value; }
    }

    public BattleScene ActiveScene
    {
        get { return mActiveScene; }
        private set { mActiveScene = value; }
    }

    public BattleFaction EnemyFaction
    {
        get { return mEnemyFaction; }
        private set { mEnemyFaction = value; }
    }

    public BattleFaction PlayerFaction
    {
        get { return mPlayerFaction; }
        private set { mPlayerFaction = value; }
    }

    public StateMechine BattleStageMechine
    {
        get { return mBattleStageMechine; }
        private set { mBattleStageMechine = value; }
    }

    public GameBattle.EStage ActiveStageType
    {
        get { return (GameBattle.EStage) BattleStageMechine.ActiveStateType; }
    }

    public BattleStageBase ActiveStage
    {
        get { return (BattleStageBase) BattleStageMechine.ActiveState; }
    }

    public int RoundCount
    {
        get { return mRoundCount; }
        set { mRoundCount = value; }
    }

    public int MaxRound
    {
        get { return mMaxRound; }
        private set { mMaxRound = value; }
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
        BattleStageMechine.ActiveState.Pause();
    }

    public void Resume()
    {
        actionState = EActionState.playing;
        BattleStageMechine.ActiveState.Resume();
    }

    public void Stop()
    {
        actionState = EActionState.stop;
        BattleStageMechine.ActiveState.Stop();
    }
    #endregion

    #region Battle
    public void ChangeStage(GameBattle.EStage stage)
    {
        BattleStageMechine.SetActiveState((int)stage);
    }

    public void OnUpdate(float deltaTime)
    {
        BattleStageMechine.OnUpdate(deltaTime);
    }


    public void OnStartBattle(StartBattleCmdReceive cmd)
    {
        if (ActiveScene == null)
            ActiveScene = new BattleScene();

        ActiveScene.Clear();

        WindowManager.instance.Create(EWindowType.LoadingWindow,
            () =>
            {
                if (ActiveScene.table.baseid != cmd.SceneID)
                {
                    ChangeStage(EStage.scene_loading);
                    ActiveScene.Switch(cmd.SceneID,
                        () =>
                        {
                            _BeginBattleLoading(cmd);
                        });
                }
                else
                {
                    _BeginBattleLoading(cmd);
                }
            });
    }


    void _BeginBattleLoading(StartBattleCmdReceive cmd)
    {
        if (BattleLoader == null)
            BattleLoader = new SceneLoader();

        BattleLoader.Reset();
  
        if (PlayerFaction == null)
            PlayerFaction = new BattleFaction();

        PlayerFaction.Clear();
        PlayerFaction.BindLoader = BattleLoader;
        PlayerFaction.Parse(cmd.PlayerFaction);

        if (EnemyFaction == null)
            EnemyFaction = new BattleFaction();

        EnemyFaction.Clear();
        EnemyFaction.BindLoader = BattleLoader;
        EnemyFaction.Parse(cmd.EnemyFaction);

        BattleLoader.CompletedCallBack = () => { ChangeStage(EStage.start); };

        ChangeStage(EStage.battle_loading);
    }
    #endregion

    #region Helper
    public AttackDefProto FindAttackDef(int id)
    {
        return AtkDefMapProto.atkDefList.Find((value) => { return value.attackDefID == id; });
    }
    #endregion
}