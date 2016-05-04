using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ProtoBuf;


public class AttackDefinition : IPoolable
{
    private AttackDefProto mProtoData = null;
    private float mCurTime = 0f;
    private BattleUnit mRealOwner = null;
    private BattleUnit mOwner = null;
    private List<BattleUnit> mHitedUnits = new List<BattleUnit>();
    private bool mOutOfData = false;
    private bool mDoHited = false;
    private int mEventIndex = 0;

    public bool OutOfData
    {
        get { return mOutOfData; }
        private set { mOutOfData = value; }
    }

    public List<BattleUnit> HitedUnits
    {
        get { return mHitedUnits; }
        private set { mHitedUnits = value; }
    }

    public BattleUnit Owner
    {
        get { return mOwner; }
        private set { mOwner = value; }
    }

    public BattleUnit RealOwner
    {
        get { return mRealOwner; }
        private set { mRealOwner = value; }
    }

    public ProtoBuf.AttackDefProto ProtoData
    {
        get { return mProtoData; }
        set { mProtoData = value; }
    }

    public void OnStart()
    {
        _ProcessSelfFx();
    }

    public void OnUpdate(float deltaTime)
    {
        if (OutOfData || ProtoData == null)
            return;

        mCurTime += deltaTime;
        if (mCurTime >= ProtoData.duration)
        {
            OutOfData = true;
            return;
        }

        _ProcessEventList(mCurTime);

        _ProcessHitedFx(mCurTime);
    }

    void _ProcessSelfFx()
    {
        PlayEffectEvent evt = ObjectPool.New<PlayEffectEvent>();
        evt.SetData(ProtoData.normalFx.SelfEffect, Owner.Model);
        GameEventManager.instance.EnQueue(evt, true);
    }

    void _ProcessHitedFx(float curTime)
    {
        if (mDoHited)
            return;

        if (curTime < ProtoData.hitedTime)
            return;

        for (int i = 0; i < HitedUnits.Count; ++i)
        {
            BattleUnit ut = HitedUnits[i];

            PlayEffectEvent efevt = ObjectPool.New<PlayEffectEvent>();
            efevt.SetData(ProtoData.normalFx.HitedEffect, ut.Model);
            GameEventManager.instance.EnQueue(efevt, true);


            PlaySoundEvent sdevt = ObjectPool.New<PlaySoundEvent>();
            sdevt.SetData(ProtoData.normalFx.HitedSound, ut.Model);
            GameEventManager.instance.EnQueue(sdevt, true);

            for (int j = 0; j < ProtoData.hitedEvents.Count; j++)
            {
                GameEventManager.instance.EnQueue(ProtoData.hitedEvents[j], true, ut.Model, null);
            }
        }

        mDoHited = true;
    }

    void _ProcessEventList(float curTime)
    {
        if (ProtoData.eventList.Count == 0)
            return;

        while (mEventIndex < ProtoData.eventList.Count)
        {
            GameEventProto efp = ProtoData.eventList[mEventIndex];
            if (efp.triggerTime > curTime)
                break;

            GameEventManager.instance.EnQueue(efp, true, Owner.Model, null);
            mEventIndex++;
        }
    }

    void _Reset()
    {
        ProtoData = null;
        RealOwner = null;
        HitedUnits.Clear();
        OutOfData = false;
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