using UnityEngine;
using System.Collections.Generic;
using DaikonForge.Tween.Interpolation;
using ProtoBuf;

public enum EGameEventType
{
    None,
    PlayEffect,
    PlaySound,
}

public abstract class GameEvent : IPoolable
{
    protected EGameEventType mType = EGameEventType.None;

    public EGameEventType Type
    {
        get { return mType; }
    }

    protected abstract void _SetType();
    protected abstract void _OnDelete();
    protected abstract void _Reset();
    public abstract void SetData(params object[] args);
    public abstract void Execute();

    void IPoolable.Create()
    {
        _Reset();
    }

    void IPoolable.New()
    {
        _Reset();
    }

    void IPoolable.Delete()
    {
        _OnDelete();
    }
}


public class GameEventManager : Singleton<GameEventManager>
{
    List<GameEvent> mGameEvents = new List<GameEvent>();
    int mCursor = 0;
    const int CURSOR_PER_FRAME = 5;  //每帧运行事件个数

    public void Update()
    {
        if (mGameEvents.Count <= 0)
            return;

        for (int i = 0; i < CURSOR_PER_FRAME; i++)
        {
            if (mCursor < mGameEvents.Count)
                mGameEvents[mCursor++].Execute();

            if (mCursor >= mGameEvents.Count)
            {
                Reset();
                break;
            }
            
        }
    }

    public void Reset()
    {
        _ClearAllEvents();
        mCursor = 0;
    }

    void _ClearAllEvents()
    {
        for (int i = 0; i < mGameEvents.Count; ++i)
        {
            GameEvent evt = mGameEvents[i];
            switch (evt.Type)
            {
                case EGameEventType.PlayEffect: { ObjectPool.Delete<PlayEffectEvent>((PlayEffectEvent)evt);  break;}
                case EGameEventType.PlaySound: { ObjectPool.Delete<PlaySoundEvent>((PlaySoundEvent)evt); break; }
            }
        }

        mGameEvents.Clear();
    }


    public void EnQueue(GameEvent gameEvent, bool insert)
    {
        if (insert)
        {
            if (mCursor > 0)
                mGameEvents[--mCursor] = gameEvent;
            else
                mGameEvents.Insert(0, gameEvent);
        }
        else
            mGameEvents.Add(gameEvent);
    }

    public void EnQueue(GameEvent gameEvent)
    {
        EnQueue(gameEvent, false);
    }

    public void EnQueue(GameEventProto proto, bool insert, GameUnit owner, GameUnit target)
    {
        GameEvent evt = null;

        switch ((EGameEventType)proto.eventType)
        {
            case EGameEventType.PlayEffect:
            {
                evt = ObjectPool.New<PlayEffectEvent>();
                evt.SetData(proto.effectData, owner); 
                break;
            }
            case EGameEventType.PlaySound:
            {
                evt = ObjectPool.New<PlaySoundEvent>();
                evt.SetData(proto.soundData, owner); 
                break;
            }
        }

        if (evt != null)
        {
            EnQueue(evt, insert);
        }

    }
}