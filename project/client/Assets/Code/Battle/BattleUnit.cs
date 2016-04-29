using UnityEngine;
using System.Collections.Generic;


public class BattleUnit : BaseGameMono, IActionControllerPlayable, IUIEventListener
{
    private EActionState mActionState = EActionState.stop;
    private CommanderUnit mModel = null;
    private float mPowerNum = 0f;
    private float mHp = 0;
    private bool mMainCommander = false;

    #region Get&Set
    public CommanderUnit Model
    {
        get { return mModel; }
        private set { mModel = value; }
    }

    public int PowerStarLevel
    {
        get { return Mathf.FloorToInt(mPowerNum/GameSetupXmlClass.instance.battle.one_star_power_val); }
    }

    public bool MainCommander
    {
        get { return mMainCommander; }
    }

    public float Hp
    {
        get { return mHp;  }
        set { mHp = value; }
    }
    #endregion

    #region Mono
    public override void Update(float deltaTime)
    {
        if (Model != null)
            Model.Update(deltaTime);
    }
    #endregion

    #region IActionControllerPlayable
    public EActionState actionState
    {
        get { return mActionState; }
        private set { mActionState = value; }
    }

    public void Pause()
    {
        actionState = EActionState.pause;
        if (Model != null)
            Model.Pause();
    }

    public void Resume()
    {
        actionState = EActionState.playing;
        if (Model != null)
            Model.Resume();
    }

    public void Stop()
    {
        actionState = EActionState.stop;
        if (Model != null)
            Model.Stop();
    }
    #endregion

    #region IUIEventListener
    void IUIEventListener.OnClick(GameObject go)
    {
    }

    void IUIEventListener.OnHover(GameObject go, bool state)
    {
    }

    void IUIEventListener.OnPress(GameObject go, bool press)
    {
    }

    void IUIEventListener.OnDragStart(GameObject go)
    {
    }

    void IUIEventListener.OnDrag(GameObject go, Vector2 delta)
    {
    }

    void IUIEventListener.OnDragEnd(GameObject go)
    {
    }
    #endregion

    #region Battle


    #endregion
}