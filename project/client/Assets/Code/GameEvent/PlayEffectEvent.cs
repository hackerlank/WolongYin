using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class PlayEffectEvent : GameEvent
{
    private EffectProto mProtoData = null;
    private GameUnit mModel = null;

    protected override void _SetType()
    {
        mType = EGameEventType.PlayEffect;
    }

    protected override void _OnDelete()
    {
    }

    protected override void _Reset()
    {
    }

    public override void SetData(params object[] args)
    {
        mProtoData = (EffectProto)args[0];
        mModel = (GameUnit) args[1];
    }

    public override void Execute()
    {
    }

}