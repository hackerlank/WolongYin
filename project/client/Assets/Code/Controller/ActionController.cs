using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class ActionController : BaseGameMono, IActionControllerPlayable
{
    private ActionGroupProto mCurrentGroup = null;
    private ActionStateProto mActiveAction = null;
    private ActionStateProto mNextAction = null;
    private bool mActionChange = false;
    private float mCurTime = 0f;
    private EActionState mActionState = EActionState.stop;
    private float mSpeed = 1f;
    private int mEventIndex = 0;
    private List<GameObject> mBindEffectList = new List<GameObject>();

    #region Get&Set
    public float Speed
    {
        get { return mSpeed;  }
        private set { mSpeed = value; }
    }

    public ActionGroupProto CurrentGroup
    {
        get { return mCurrentGroup; }
    }

    public ActionStateProto ActiveAction
    {
        get { return mActiveAction; }
    }
    #endregion

    #region mono funs
    public override void Update(float deltaTime)
    {
        if (actionState == EActionState.stop)
            return;

        mCurTime += deltaTime;

        if (mActionChange)
        {
            mActiveAction = mNextAction;
            _Reset();
            mActionChange = false;
            mNextAction = null;
        }

        if (mActiveAction != null)
        {
            _TickAction(mCurTime);
        }
    }
    #endregion

    #region action control funs
    public void Play(int actionId)
    {
        if (CurrentGroup == null)
            return;

        int idx = ActionController.FindActionIndex(CurrentGroup, actionId);
        if (idx < 0)
        {
            Logger.instance.Error("动作组找不到动作ID: {0}, 角色： {1}\n", actionId, this.GetGameUnit().TableID);
            return;
        }

        _ChangeAction(idx);
    }

    public void Play(ActionStateProto action)
    {
        if (action.slotList.Count == 0)
            return;

        AnimSlotProto animSlot = action.slotList[UnityEngine.Random.Range(0, action.slotList.Count - 1)];

        float btime = 0f;//action.BlendTime * 0.001f;
        float ntime = animSlot.startTime;
        float ctime = action.stateTime / (animSlot.endTime - animSlot.startTime);

        this.GetGameUnit().CrossFade(animSlot.animName, btime, ntime);
    }

    public void SetPlaybackSpeed(float speed)
    {
        Speed = speed;

        GameUnit ut = this.GetGameUnit();
        if (ut == null)
            return;
        
        if (ut.animatorController != null)
        {
            ut.animatorController.SetPlaybackSpeed(speed);
        }
    }

    #endregion

    #region private funs
    protected virtual void _TickAction(float curTime)
    {
        _ProcessEventList(curTime);

        if (curTime > ActiveAction.stateTime)
        {
            _ProcessTickFinish();
        }
    }

    protected void _ProcessEventList(float curTime)
    {
        if (ActiveAction.eventList.Count == 0)
            return;

        GameUnit model = this.GetGameUnit();
        while (mEventIndex < ActiveAction.eventList.Count)
        {
            GameEventProto efp = ActiveAction.eventList[mEventIndex];
            if (efp.triggerTime > curTime)
                break;

            GameEventManager.instance.EnQueue(efp, true, model, null);
            mEventIndex++;
        }
    }

    protected void _ProcessTickFinish()
    {
        Stop();
        ClearBindEffect();
    }

    void _Reset()
    {
        mCurTime = 0f;
        mEventIndex = 0;
        ClearBindEffect();
    }

    void _ChangeAction(int idx)
    {
        ActionStateProto oldAction = mActiveAction;
        mNextAction = CurrentGroup.actions[idx];

        //mNextAction.EventList.Sort(delegate(ActionEventData a, ActionEventData b) { return a.TriggerTime.CompareTo(b.TriggerTime); });

        mActionChange = true;

        actionState = EActionState.playing;
    }
    #endregion

    #region helper
    public static int FindActionIndex(ActionGroupProto data, int stateId)
    {
        if (data == null)
            return -1;

        for (int i = 0; i < data.actions.Count; ++i)
        {
            if (data.actions[i].stateID == stateId)
            {
                return i;
            }
        }

        return -1;
    }
    #endregion
 
    #region IActionControllerPlayable
    public EActionState actionState
    {
        get { return mActionState; }
        private set { mActionState = value; }
    }


    public void Resume()
    {
        actionState = EActionState.playing;
        Speed = 1f;
    }

    public void Stop()
    {
        actionState = EActionState.stop;
    }

    public void Pause()
    {
        actionState = EActionState.pause;
        Speed = 0f;
    }
    #endregion

    #region effect list
    public void AddBindEffect(GameObject go)
    {
        mBindEffectList.Add(go);
    }

    public void ClearBindEffect()
    {
        for (int i = 0; i < mBindEffectList.Count; ++i)
        {
            GameObject go = mBindEffectList[i];
            if (go == null)
                continue;

            Utility.ReturnToPool(go);
        }
    }
    #endregion
}