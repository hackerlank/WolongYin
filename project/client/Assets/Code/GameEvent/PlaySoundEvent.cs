using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;

public class PlaySoundEvent : GameEvent
{
    private SoundProto mProto;
    private GameUnit mModel;

    protected override void _SetType()
    {
        mType = EGameEventType.PlaySound;
    }

    protected override void _OnDelete()
    {
    }

    protected override void _Reset()
    {
        mProto = null;
        mModel = null;
    }

    public override void SetData(params object[] args)
    {
        mProto = (SoundProto) args[0];
        mModel = (GameUnit) args[1];
    }

    public override void Execute()
    {
        if (mProto == null || mModel == null)
            return;
        
        FMOD_StudioSystem.instance.PlayOneShot(mProto.assetName, mModel.position);
    }
}