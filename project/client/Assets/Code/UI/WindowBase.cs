using UnityEngine;
using System.Collections.Generic;

public abstract class WindowBase
{
    protected GameObject mWindowObject = null;
    protected EWindowType mWinType = EWindowType.none;
    protected WindowData mDefineData = null;
    protected Dictionary<string, GameObject> mBindingObjectMap = new Dictionary<string, GameObject>();
    private int mMinDepth = 0;
    private int mMaxDepth = 0;
    private bool mIsOpened = false;
    private ResourceCenter.Handle mCachedHandle = null;
    private UITweener[] mTweeners = null;

    #region fields
    public ResourceCenter.Handle CachedHandle
    {
        get { return mCachedHandle; }
        set { mCachedHandle = value; }
    }
    public bool IsOpened
    {
        get { return mIsOpened; }
    }
    public EWindowType WinType
    {
        get { return mWinType; }
    }
    public WindowData DefineData
    {
        get { return mDefineData; }
        set { mDefineData = value; }
    }
    public UnityEngine.GameObject WindowObject
    {
        get { return mWindowObject; }
    }
    public int MinDepth
    {
        get { return mMinDepth; }
        set { mMinDepth = value; }
    }
    public int MaxDepth
    {
        get { return mMaxDepth; }
        set { mMaxDepth = value; }
    }
    #endregion

    public void Init(EWindowType type, GameObject winObj)
    {
        mWindowObject = winObj;
        mWinType = type;
        mTweeners = mWindowObject.GetComponentsInChildren<UITweener>(true);

        OnRegisterComponents();

        OnInit();
    }

    public void Open(params object[] Parameters)
    {
        if (IsOpened)
            return;

        BeforeOpen();

        WindowManager.instance.PushActiveWindow(this);
        mWindowObject.SetActive(true);

        if (mTweeners != null)
        {
            foreach (UITweener tw in mTweeners)
            {
                tw.ResetToBeginning();
                tw.PlayForward();
            }
        }

        OnOpen(Parameters);

        mIsOpened = true;
    }

    public void Update(float deltaTime)
    {
        OnUpdate(deltaTime);
    }

    public void Close()
    {
        mIsOpened = false;
        mWindowObject.SetActive(false);
        WindowManager.instance.OnCloseWindow(this);
        OnClose();
    }

    public void Destroy()
    {
        ResourceCenter.instance.BreakLoadObject(DefineData.AssetName,
            CachedHandle);

        OnDestory();

        WindowManager.instance.DestoryWindow(this);
    }

    #region virtual func
    protected virtual void OnRegisterComponents() { }
    protected virtual void OnInit() { }
    protected virtual void BeforeOpen() { }
    protected virtual void OnOpen(params object[] Parameters) { }
    protected virtual void OnClose() { }
    protected virtual void OnUpdate(float deltaTime) { }
    protected virtual void OnDestory() { }
    protected virtual void OnClick(string path, GameObject gameObj) { }
    protected virtual void OnPress(string path, GameObject gameObj, bool bPress) { }
    protected virtual void OnDBClick(string path, GameObject gameObj) { }
    protected virtual void OnDrag(string path, GameObject go, Vector2 delta) { }
    protected virtual void OnTooltip(string path, GameObject go, bool state) { }
    #endregion

    #region helper func
    protected GameObject RegisterComponent(string path)
    {
        if (mWindowObject == null)
        {
            Logger.instance.Error("窗口加载出错！  窗口名： {0}  窗口资源: {1}!\n", DefineData.WindowName, DefineData.AssetName);
            return null;
        }

        if (mBindingObjectMap.ContainsKey(path))
        {
            Logger.instance.Error("重复的节点路径 : {0}   窗口名： {1}  窗口资源: {2}!\n", path, DefineData.WindowName, DefineData.AssetName);
            return null;
        }

        GameObject go = null;

        Transform node = mWindowObject.transform.Find(path);
        if (node == null)
        {
            go = Utility.FindNode(mWindowObject, path);
            if (go == null)
            {
                Logger.instance.Error("找不到节点 {0}  窗口名： {1}  窗口资源: {2}!\n", path, DefineData.WindowName, DefineData.AssetName);
                return null;
            }
        }
        else
        {
            go = node.gameObject;
        }


        mBindingObjectMap.Add(path, go);
        SetCallback(path, go);

        return go;
    }
    protected GameObject GetComponent(string path)
    {
        GameObject go = GetChildObject(path);
        return go;
    }
    protected T GetComponent<T>(string path) where T : Component
    {
        GameObject go = GetChildObject(path);
        return go != null ? go.GetComponent<T>() : null;
    }


    private void SetCallback(string path, GameObject obj)
    {
        if (obj.GetComponent<Collider>() == null)
        {
            Logger.instance.Error("UI控件没有碰撞框 {0}  窗口名： {1}  窗口资源: {2}!\n", obj.name, DefineData.WindowName, DefineData.AssetName);
            return;
        }

        UIEventListener eventListen = UIEventListener.Get(obj);
        eventListen.onClick = (GameObject go) => { OnClick(path, obj); };
        eventListen.onPress = (GameObject go, bool bPress) => { OnPress(path, obj, bPress); };
        eventListen.onDoubleClick = (GameObject go) => { OnDBClick(path, obj); };
        eventListen.onDrag = (GameObject go, Vector2 delta) => { OnDrag(path, obj, delta); };
        eventListen.onTooltip = (GameObject go, bool state) => { OnTooltip(path, obj, state); };
    }
    private GameObject GetChildObject(string path)
    {
        GameObject go = null;
        if (mBindingObjectMap.TryGetValue(path, out go))
            return go;

        Transform node = mWindowObject.transform.Find(path);
        if (node == null)
        {
            go = Utility.FindNode(mWindowObject, path);
        }
        else
        {
            go = node != null ? node.gameObject : null;
        }

        if (go == null)
        {
            Logger.instance.Error("找不到节点 {0}  窗口名： {1}  窗口资源: {2}!\n", path, DefineData.WindowName, DefineData.AssetName);
        }
        return go;
    }
    #endregion
}

public abstract class SingletonWindow<T> : WindowBase where T : WindowBase
{
    private static T ms_instance = null;

    public static T Get()
    {
        if (ms_instance == null)
        {
            Logger.instance.Error("窗口 {0} 还没有创建!\n", typeof(T).ToString());
        }

        return ms_instance;
    }

    protected SingletonWindow()
        : base()
    {
        ms_instance = this as T;
    }
}
