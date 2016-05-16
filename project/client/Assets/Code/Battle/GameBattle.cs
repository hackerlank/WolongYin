using UnityEngine;
using System.Collections.Generic;


public class GameBattle : Singleton<GameBattle>, IActionControllerPlayable
{
    public enum EStage
    {
        awake,
        start,
        end,
        round_start,
        round_end,
        round_playing,
    }

    private int mRoundCount = 0;
    private int mMaxRound = 1;
    private EActionState mActionState = EActionState.stop;
    private StateMechine mBattleStageMechine = new StateMechine();
    private BattleFaction mPlayerFaction = null;
    private BattleFaction mEnemyFaction = null;

    #region Get&Set
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
        private set { mRoundCount = value; }
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

    public void OnUpdate(float deltaTime)
    {
        BattleStageMechine.OnUpdate(deltaTime);
    }

}