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
    private bool mOutOfData = false;
    private bool mDoHitedFx = false;
    private bool mDoHit = false;
    private int mEventIndex = 0;
    private BattleSkill mSkillData = null;

    #region Get&Set
    public BattleSkill SkillData
    {
        get { return mSkillData; }
        private set { mSkillData = value; }
    }

    public bool OutOfData
    {
        get { return mOutOfData; }
        private set { mOutOfData = value; }
    }

    public BattleUnit Owner
    {
        get { return mOwner; }
        set { mOwner = value; }
    }

    public BattleUnit RealOwner
    {
        get { return mRealOwner; }
        set { mRealOwner = value; }
    }

    public ProtoBuf.AttackDefProto ProtoData
    {
        get { return mProtoData; }
        set { mProtoData = value; }
    }
    #endregion

    public void OnStart()
    {
        mCurTime = 0f;
        OutOfData = false;
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

        _ProcessHit(mCurTime);
    }

    void _ProcessHit(float curTime)
    {
        if (mDoHit || ProtoData.hitedData == null)
            return;

        if (curTime < ProtoData.hitedData.triggerTime)
            return;

        for (int i = 0; i < SkillData.HitedUnits.Count; ++i)
        {
            BattleUnit ut = SkillData.HitedUnits[i];
            ut.OnHited(RealOwner, this);
        }

        mDoHit = true;
    }

    void _ProcessSelfFx()
    {
        PlayEffectEvent evt = ObjectPool.New<PlayEffectEvent>();
        evt.SetData(ProtoData.normalFx.SelfEffect, Owner.Model);
        GameEventManager.instance.EnQueue(evt, true);
    }

    void _ProcessHitedFx(float curTime)
    {
        if (mDoHitedFx)
            return;

        if (curTime < ProtoData.hitedTime)
            return;

        for (int i = 0; i < SkillData.HitedUnits.Count; ++i)
        {
            BattleUnit ut = SkillData.HitedUnits[i];

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

        mDoHitedFx = true;
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
        Owner = null;
        RealOwner = null;
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