using UnityEngine;
using System.Collections.Generic;


public class WindowManager : SingletonMonoBehavior<WindowManager>
{
    public Transform UIRoot = null;
    public UICamera NGUICamera = null;

    [SerializeField]
    private List<WindowBase> mOpenList = new List<WindowBase>();

    [SerializeField]
    private List<WindowBase> mInitedList = new List<WindowBase>();

    [SerializeField]
    private WindowBase mActiveWindow = null;

    [SerializeField]
    private WindowBase mLastWindow = null;

    public static void Build()
    {
        if (WindowManager.instance != null)
        {
            Object.Destroy(WindowManager.instance.gameObject);
        }

        GameObject go = new GameObject("_WindowMgr");
        go.AddComponent<WindowManager>();
        DontDestroyOnLoad(go);

        //WindowManager.instance.CreateUIRoot();

        WindowManager.instance.UIRoot = ClientRoot.instance.nguiRoot.transform;
        WindowManager.instance.NGUICamera = ClientRoot.instance.uiCamera.gameObject.GetComponent<UICamera>();
    }

    void Update()
    {
        for (int i = 0; i < mOpenList.Count; ++i)
        {
            mOpenList[i].Update(Time.deltaTime);
        }
    }

    public void ClearInStage()
    {
        for (int i = 0; i < mInitedList.Count; ++i)
        {
            WindowBase win = mInitedList[i];
            if (win.WinType != EWindowType.LoadingWindow)
            {
                win.Destroy();
            }
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < mInitedList.Count; ++i)
        {
            WindowBase win = mInitedList[i];
            Utility.Destroy(win.WindowObject);
        }
        mInitedList.Clear();
        mOpenList.Clear();
        mActiveWindow = mLastWindow = null;
    }

    public void DestoryWindow(WindowBase win)
    {
        if (win == null)
            return;

        if (mActiveWindow == win)
            win.Close();

        bool b = mInitedList.Remove(win);
        if (b)
        {
            string assetName = win.DefineData.AssetName;
            Utility.Destroy(win.WindowObject);
            ResourceCenter.instance.RemoveAsset(assetName, ResourceCenter.AssetType.normal);
        }
    }

    public WindowBase Create(EWindowType type, Utility.VoidDelegate callback)
    {
        WindowData data = null;
        if (!WindowFact.instance.sWindowTypeMap.TryGetValue(type, out data))
        {
            Logger.instance.Error("没有注册的窗口类型 ：{0}!\n", type);
            return null;
        }

        WindowBase old = GetWindow<WindowBase>(type);
        if (old != null)
        {
            if (callback != null)
                callback();
            return old;
        }

        WindowBase win = (WindowBase)System.Activator.CreateInstance(data.ClassType);
        win.DefineData = data;
        mInitedList.Add(win);

        win.CachedHandle = ResourceCenter.instance.LoadObject(data.AssetName,
                            (value) =>
                            {
                                OnLoadedWindow(type, win, value);
                                if (callback != null)
                                    callback();
                            });
        return win;
    }

    public void OpenWindow(EWindowType type, params object[] args)
    {
        WindowBase win = GetWindow<WindowBase>(type);
        if (win != null)
        {
            win.Open(args);
        }
    }

    public void CloseWindow(EWindowType type)
    {
        WindowBase win = GetWindow<WindowBase>(type);
        if (win != null)
        {
            win.Close();
        }
    }


    public WindowBase CreateWindowInScene(EWindowType type, Utility.VoidDelegate callback, SceneLoader loader)
    {
        WindowData data = null;
        if (!WindowFact.instance.sWindowTypeMap.TryGetValue(type, out data))
        {
            Logger.instance.Error("没有注册的窗口类型 ：{0}!\n", type);
            return null;
        }

        WindowBase old = GetWindow<WindowBase>(type);
        if (old != null)
            return old;

        WindowBase win = (WindowBase)System.Activator.CreateInstance(data.ClassType);
        win.DefineData = data;
        mInitedList.Add(win);

        loader.Push(data.AssetName, ResourceCenter.AssetType.normal,
            (value) =>
            {
                OnLoadedWindow(type, win, value);
                if (callback != null)
                    callback();
            });

        return win;
    }

    public void PushActiveWindow(WindowBase win)
    {
        mLastWindow = mActiveWindow;
        mActiveWindow = win;
        ProcessWindowDepth(win);
        mOpenList.Add(win);
    }

    public void OnCloseWindow(WindowBase win)
    {
        mOpenList.Remove(win);
        if (mActiveWindow == win)
        {
            mActiveWindow = mLastWindow;
        }
        if (mOpenList.Count > 1)
        {
            mLastWindow = mOpenList[mOpenList.Count - 2];
        }
    }

    public T GetWindow<T>(EWindowType type) where T : WindowBase
    {
        for (int i = 0; i < mInitedList.Count; ++i)
        {
            if (mInitedList[i].WinType == type)
            {
                return mInitedList[i] as T;
            }
        }

        return null;
    }

    private void ProcessWindowDepth(WindowBase newWin)
    {
        int imin = mOpenList.Count == 0 ? 0 : mOpenList[mOpenList.Count - 1].MaxDepth;
        imin += 1;
        int imax = imin;
        UIPanel[] panels = newWin.WindowObject.GetComponentsInChildren<UIPanel>(true);
        for (int i = 0; i < panels.Length; ++i)
        {
            int dp = panels[i].depth;
            int cdp = imin + dp;
            panels[i].depth = cdp;
            if (imax < cdp)
                imax = cdp;
        }
        newWin.MinDepth = imin;
        newWin.MaxDepth = imax;
    }

    private void OnLoadedWindow(EWindowType type, WindowBase win, Object asset)
    {
        GameObject go = (GameObject)Object.Instantiate(asset);
        Utility.SetIdentityChild(UIRoot, go.transform);

        if (Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.OSXEditor)
        {
            Utility.ResetShader(go);
        }

        win.Init(type, go);

        go.SetActive(false);
    }

    private void CreateUIRoot()
    {
        UIRoot root = Object.FindObjectOfType<UIRoot>();
        if (root == null)
        {
            Object rAsset = Resources.Load("misc/UI Root", typeof(GameObject));
            GameObject rgo = (GameObject)Object.Instantiate(rAsset);
            rgo.name = rAsset.name;
            this.UIRoot = rgo.transform;
        }
        else
        {
            this.UIRoot = root.transform;
        }

        NGUICamera = this.UIRoot.gameObject.GetComponentInChildren<UICamera>();
        DontDestroyOnLoad(this.UIRoot.gameObject);
    }

}

