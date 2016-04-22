using System.Collections.Generic;


public interface IStage
{
    void OnEnter();
    void OnExit();
    void OnUpdate(float deltaTime);
    void OnGUI();
}

public class StageManager : Singleton<StageManager>
{
    private IStage activeState = null;
    private Dictionary<int, IStage> stMap = new Dictionary<int, IStage>();

    public int StateCount { get { return stMap.Count; } }
    public IStage ActiveState { get { return activeState; } }

    public void Initialize()
    {
        StageManager.instance.Register((int)GameDef.EGameStage.startup_stage, new GameStartStage());
    }

    public void Register(int type, IStage st)
    {
        if (st == null || type <= 0) return;

        UnRegister(type);
        stMap.Add(type, st);
    }

    public void UnRegister(int type)
    {
        if (type <= 0) return;

        if (stMap.ContainsKey(type))
            stMap.Remove(type);
    }

    public IStage GetState(int type)
    {
        if (!stMap.ContainsKey(type)) return null;

        return stMap[type];
    }

    public void SetActiveState(int type, Utility.VoidDelegate clearHandle = null)
    {
        if (!stMap.ContainsKey(type)) return;

        if (activeState != null) activeState.OnExit();

        if (clearHandle != null) clearHandle();

        activeState = stMap[type];
        if (activeState != null) activeState.OnEnter();
    }

    public void OnUpdate(float deltaTime)
    {
        if (activeState != null)
            activeState.OnUpdate(deltaTime);
    }

    public void Clear()
    {
        stMap.Clear();
        activeState = null;
    }
}

