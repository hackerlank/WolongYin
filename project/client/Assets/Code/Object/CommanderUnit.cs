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

    public override void CrossFade(string name, float blendtime = 0.3f, float normalizedTime = 0f)
    {
        for (int i = 0; i < mSoliders.Count; ++i)
        {
            mSoliders[i].CrossFade(name, blendtime, normalizedTime);
        }
    }
}
