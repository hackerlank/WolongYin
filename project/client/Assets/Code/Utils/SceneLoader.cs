using UnityEngine;
using System.Collections.Generic;


public class SceneLoader
{
    struct SceneAsset
    {
        public string ResName;
        public ResourceCenter.AssetType Type;
        public ResourceCenter.Handle Handle;

        public SceneAsset(string name)
        {
            ResName = name;
            Type = ResourceCenter.AssetType.normal;
            Handle = null;
        }

        public SceneAsset(string name, ResourceCenter.AssetType type)
        {
            ResName = name;
            Type = type;
            Handle = null;
        }

        public SceneAsset(string name, ResourceCenter.AssetType type, ResourceCenter.Handle handle)
        {
            ResName = name;
            Type = type;
            Handle = handle;
        }
    };

    private List<SceneAsset> mToLoadList = new List<SceneAsset>();
    private int mLoadedCount = 0;
    private int mTotalCount = 0;
    private bool mDone = false;
    //private AsyncOperation mAsyncOpt = null;
    private Utility.VoidDelegate mCompleted = null;

    public float Progress
    {
        get 
        {
            if (mTotalCount == 0)
            {
                return Done ? 1 : 0;
            }
            return mLoadedCount / (float)mTotalCount; 
        }
    }

    public Utility.VoidDelegate CompletedCallBack
    {
        set { mCompleted = value; }
    }

    public bool Done
    {
        get { return mDone; }
    }

    public void Reset()
    {
        //mAsyncOpt = null;
        mLoadedCount = 0;
        mTotalCount = 0;
        mDone = false;
        mToLoadList.Clear();
    }

    public void Push(string name,
        ResourceCenter.AssetType type = ResourceCenter.AssetType.normal,
        ResourceCenter.Handle handle = null)
    {
        if (string.IsNullOrEmpty(name))
            return;

        //Debug.Log(string.Format("push : {0}\n", name));
        int idx = mToLoadList.FindIndex((SceneAsset l) => { return name == l.ResName && type == l.Type; });
        if (idx == -1)
        {
            mToLoadList.Add(new SceneAsset(name, type, handle));
            mDone = false;
        }
    }


    public void Update()
    {
        if (mDone)
            return;

        if (mToLoadList.Count > 0)
        {
            mTotalCount = mToLoadList.Count;
            for (int i = 0; i < mTotalCount/*mToLoadList.Count*/; ++i)
            {
                SceneAsset s = mToLoadList[i];
                RequestAsset(s);
            }
            mToLoadList.Clear();
        }

        //             if (mAsyncOpt != null && mAsyncOpt.isDone)
        //             {
        //                 OnLevelWasLoaded();
        //             }

    }

    private void RequestAsset(SceneAsset asset)
    {
        switch (asset.Type)
        {
            case ResourceCenter.AssetType.normal:
                {
                    ResourceCenter.instance.LoadObject(asset.ResName,
                        (Object obj) =>
                        {
                            OnLoaded(asset, obj);
                        });
                    break;
                }
            case ResourceCenter.AssetType.scene:
                {
                    ResourceCenter.instance.LoadScene(asset.ResName,
                        (Object obj) =>
                        {
                            OnLoaded(asset, obj);
                        });
                    break;
                }
            case ResourceCenter.AssetType.additive_scene:
                {
                    ResourceCenter.instance.LoadAdditiveScene(asset.ResName,
                        (Object obj) =>
                        {
                            OnLoaded(asset, obj);
                        });
                    break;
                }
            case ResourceCenter.AssetType.audio:
                {
                    ResourceCenter.instance.LoadAudio(asset.ResName,
                        (Object obj) =>
                        {
                            OnLoaded(asset, obj);
                        });
                    break;
                }
            case ResourceCenter.AssetType.texture:
                {
                    ResourceCenter.instance.LoadTexture(asset.ResName,
                        (Object obj) =>
                        {
                            OnLoaded(asset, obj);
                        });
                    break;
                }
            default:
                {
                    Logger.instance.Error("Unprocess AssetType : {0} : {1}\n", asset.Type, asset.ResName);
                    break;
                }
        }
    }

    private void OnLoaded(SceneAsset asset, Object obj)
    {
        //Debug.Log(string.Format("OnLoaded {0} \n", asset.ResName));
        //if (asset.Type != ResourceCenter.AssetType.scene)
        {
            ++mLoadedCount;
            if (asset.Handle != null)
                asset.Handle(obj);

            CheckDone();
        }
        //             else
        //             {
        //                 mAsyncOpt = Application.LoadLevelAsync(asset.ResName);
        //             }
    }

    //         private void OnLevelWasLoaded()
    //         {
    //             ++mLoadedCount;
    //             mAsyncOpt = null;
    // 
    //             CheckDone();
    //         }


    private void CheckDone()
    {
        if (mLoadedCount >= mTotalCount)
        {
            //mAsyncOpt = null;
            mLoadedCount = 0;
            mTotalCount = 0;
            mDone = true;

            if (mCompleted != null)
                mCompleted();
        }
    }
}
