using UnityEngine;
using System.Collections.Generic;


public class GameBattle : Singleton<GameBattle>, IActionControllerPlayable
{
    public enum EState
    {
        awake,
        start,
        end,
        round_start,
        round_end,
        round_playing,
    }


    private EState mStage = EState.awake;
    private EActionState mActionState = EActionState.stop;
    private int mRoundCount = 0;
    private int mMaxRound = 1;

    #region Get&Set
    public GameBattle.EState Stage
    {
        get { return mStage; }
        private set { mStage = value; }
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
    }

    public void Resume()
    {
        actionState = EActionState.playing;
    }

    public void Stop()
    {
        actionState = EActionState.stop;
    }
    #endregion

    public void OnUpdate(float deltaTime)
    {
        _UpdateStage(deltaTime);
    }

    void _UpdateStage(float deltaTime)
    {
        switch (Stage)
        {
            case EState.awake:
                {
                    break;
                }
            case EState.start:
                {
                    break;
                }
            case EState.end:
                {
                    break;
                }
            case EState.round_start:
                {
                    break;
                }
            case EState.round_end:
                {
                    if (RoundCount == MaxRound)
                    {
                        Stage = EState.end;
                    }
                    else
                    {
                        Stage = EState.round_start;
                    }
                    break;
                }
            case EState.round_playing:
                {
                    break;
                }
        }
    }
}