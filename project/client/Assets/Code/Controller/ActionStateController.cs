using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;


public class ActionStateController : BaseGameMono, IActionControllerPlayable
{
    private ActionGroupProto mCurrentGroup = null;
    private ActionStateProto mActiveAction = null;
    private ActionStateProto mNextAction = null;
    private bool mActionChange = false;
    private float mTotalTime = 0f;
    private EActionState mActionState = EActionState.stop;
    private float mSpeed = 1f;
    private SkillTable mCurrentSkill = null;
    private int mEventIndex = 0;

    #region Get&Set
    public SkillTable CurrentSkill
    {
        get { return mCurrentSkill; }
        set { mCurrentSkill = value; }
    }

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
        
        int preTime = (int)mTotalTime;
        mTotalTime = (mTotalTime + (Time.deltaTime * 1000 * Speed)) % 9000000; // 避免负数
        if (preTime > mTotalTime)
            preTime = 0;

        int curTime = (int)mTotalTime;

        if (mActionChange)
        {
            mActiveAction = mNextAction;
            _Reset();
            mActionChange = false;
            mNextAction = null;
        }

        if (mActiveAction != null)
        {
            _TickAction(curTime);
        }
    }
    #endregion

    #region action control funs
    public void PlayActionState(int stateId)
    {
        if (CurrentGroup == null)
            return;

        int idx = ActionStateController.GetActionStateIndex(CurrentGroup, stateId);
        if (idx < 0)
        {
            Logger.instance.Error("动作组找不到动作ID: {0}, 角色： {1}\n", stateId, this.GetGameUnit().TableID);
            return;
        }

        _ChangeAction(idx);
    }

    public void PlayActionState(ActionStateProto action)
    {
        if (action.slotList.Count == 0)
            return;

        AnimSlotProto animSlot = action.slotList[UnityEngine.Random.Range(0, action.slotList.Count - 1)];

        float btime = 0f;//action.BlendTime * 0.001f;
        float ntime = animSlot.startTime * 0.01f;
        float ctime = (float)action.stateTime * 0.001f / ((animSlot.endTime - animSlot.startTime) * 0.01f);

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
    void _TickAction(int curTime)
    {
        _ProcessEventList(curTime);

        if (curTime > ActiveAction.stateTime)
        {
            _ProcessTickFinish();
        }
    }

    void _ProcessEventList(int curTime)
    {
        if (ActiveAction.eventList.Count == 0)
            return;

        GameUnit model = this.GetGameUnit();
        while (mEventIndex < ActiveAction.eventList.Count)
        {
            GameEventProto efp = ActiveAction.eventList[mEventIndex];
            if (efp.triggerTime > curTime)
                break;

            PlayEffectEvent evt = ObjectPool.New<PlayEffectEvent>();
            evt.SetData(efp, model);
            GameEventManager.instance.EnQueue(evt, true);
            mEventIndex++;
        }
    }

    void _ProcessTickFinish()
    {
        Stop();
    }

    void _Reset()
    {
        mTotalTime = 0f;
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
    public static int GetActionStateIndex(ActionGroupProto data, int stateId)
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
}