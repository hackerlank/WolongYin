using System.Collections.Generic;


public interface IStage
{
    void OnEnter();
    void OnExit();
    void OnUpdate(float deltaTime);
    void OnGUI();
    void Pause();
    void Resume();
    void Stop();
}

public class StateMechine
{
    private IStage mActiveState = null;
    private int mActiveStateType = -1;

    private Dictionary<int, IStage> mStateMap = new Dictionary<int, IStage>();

    public int StateCount { get { return mStateMap.Count; } }
    public IStage ActiveState { get { return mActiveState; } }
    public int ActiveStateType
    {
        get { return mActiveStateType; }
        private set { mActiveStateType = value; }
    }

    public void Register(int type, IStage st)
    {
        if (st == null || type <= 0) return;

        UnRegister(type);
        mStateMap.Add(type, st);
    }

    public void UnRegister(int type)
    {
        if (type <= 0) return;

        if (mStateMap.ContainsKey(type))
            mStateMap.Remove(type);
    }

    public IStage GetState(int type)
    {
        if (!mStateMap.ContainsKey(type)) return null;

        return mStateMap[type];
    }

    public void SetActiveState(int type, Utility.VoidDelegate clearHandle = null)
    {
        if (!mStateMap.ContainsKey(type)) return;

        if (mActiveState != null) mActiveState.OnExit();

        if (clearHandle != null) clearHandle();

        mActiveState = mStateMap[type];
        if (mActiveState != null) mActiveState.OnEnter();
        ActiveStateType = type;
    }

    public void OnUpdate(float deltaTime)
    {
        if (mActiveState != null)
            mActiveState.OnUpdate(deltaTime);
    }

    public void Clear()
    {
        mStateMap.Clear();
        mActiveState = null;
    }
}

public class AppStageManager : StateMechine
{
    #region instance
    protected static readonly AppStageManager ms_instance = new AppStageManager();

    protected AppStageManager()
    {
    }

    public static AppStageManager instance
    {
        get { return ms_instance; }
    }

    #endregion


    public void Initialize()
    {
        AppStageManager.instance.Register((int) GameDef.EGameStage.startup_stage, new GameStartStage());
    }
}

