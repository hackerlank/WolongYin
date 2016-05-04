using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class HitDefinition : IPoolable
{
    private AttackDefProto mProtoData = null;
    private float mCurTime = 0f;

    public ProtoBuf.AttackDefProto ProtoData
    {
        get { return mProtoData; }
        set { mProtoData = value; }
    }

    public void OnUpdate(float deltaTime)
    {
        mCurTime += deltaTime;
    }

    void IPoolable.Create()
    {
    }

    void IPoolable.New()
    {
    }

    void IPoolable.Delete()
    {
    }
}