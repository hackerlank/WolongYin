using UnityEngine;
using System.Collections.Generic;


public enum EGameEventType
{
    None,
    PlayEffect,
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
    int mMaxCursor = 20;  //缓存事件总数量
    int mCursorPerFrame = 5;  //每帧运行事件个数

    public void Update()
    {
        for (int i = 0; i < mCursorPerFrame; i++)
        {
            if (mGameEvents.Count > 0)
            {
                if (mCursor < mGameEvents.Count)
                    mGameEvents[mCursor++].Execute();

                if (mCursor >= mGameEvents.Count)
                {
                    mGameEvents.Clear();
                    mCursor = 0;
                }
            }
        }
    }

    public void Reset()
    {
        mGameEvents.Clear();
        mCursor = 0;
    }


    public void EnQueue(GameEvent gameEvent, bool insert)
    {
        if (mGameEvents.Count > mMaxCursor)
            return;

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

    public void EnQueue(EGameEventType type, bool insert, params object[] args)
    {
        GameEvent evt = null;

        switch (type)
        {
            case EGameEventType.PlayEffect:
            {
                evt = ObjectPool.New<PlayEffectEvent>();
                break;
            }
        }

        if (evt != null)
        {
            evt.SetData(args); 
            EnQueue(evt, insert);
        }

    }
}