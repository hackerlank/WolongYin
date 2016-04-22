using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

public class CommanderUnit : GameUnit
{
    private CommanderProto mData = null;
    private List<SoliderUnit> mSoliders = new List<SoliderUnit>(); 

    public CommanderProto Data
    {
        get { return mData; }
    }

    public override void ChangeModel(string model)
    {
        // empty, no process
    }

    public override void Update(float deltaTime)
    {
        for (int i = 0; i < mSoliders.Count; ++i)
        {
            mSoliders[i].Update(deltaTime);
        }
    }

    public override void OnDestroy()
    {
        for (int i = 0; i < mSoliders.Count; ++i)
        {
            mSoliders[i].OnDestroy();
        }
    }

    #region IActionControllerPlayable
    public override void CrossFade(string name, float blendtime = 0.3f, float normalizedTime = 0f)
    {
        actionState = EActionState.playing;
        for (int i = 0; i < mSoliders.Count; ++i)
        {
            mSoliders[i].CrossFade(name, blendtime, normalizedTime);
        }
    }

    public override void Stop()
    {
        actionState = EActionState.stop;
        for (int i = 0; i < mSoliders.Count; ++i)
        {
            mSoliders[i].Stop();
        }
    }

    public override void Pause()
    {
        actionState = EActionState.pause;
        for (int i = 0; i < mSoliders.Count; ++i)
        {
            mSoliders[i].Pause();
        }
    }

    public override void Resume()
    {
        actionState = EActionState.playing;
        for (int i = 0; i < mSoliders.Count; ++i)
        {
            mSoliders[i].Resume();
        }
    }
    #endregion
}
