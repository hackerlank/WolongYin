using UnityEngine;
using System.Collections.Generic;

public class GameSceneBase
{
    protected SceneBase mtable = null;
    protected SceneLoader mloader = new SceneLoader();

    protected bool mDone = false;
    protected FMOD.Studio.EventInstance mSceneBgm = null;

    public SceneLoader Loader
    {
        get { return mloader; }
    }

    public bool Done
    {
        get { return mDone; }
        set { mDone = value; }
    }

    public FMOD.Studio.EventInstance SceneBgm
    {
        get { return mSceneBgm; }
        set { mSceneBgm = value; }
    }

    public SceneBase table
    {
        get { return mtable; }
        set { mtable = value; }
    }

    protected virtual void _OnSceneInit()
    {

    }

    protected virtual void _OnSceneWasLoaded()
    {

    }

    protected virtual void _OnUpdate(float deltaTime)
    {

    }

    protected virtual void _OnClear()
    {

    }

    public void Switch(int sid, Utility.VoidDelegate OnLoadedCallback)
    {
        SceneBase tb = SceneBaseManager.instance.Find((uint)sid);
        if (tb == null)
        {
            Logger.instance.Error("找不到场景配置 ： {0} !\n", sid);
            return;
        }

        table = tb;
        Done = false;

        mloader.Reset();
        mloader.Push(table.name, ResourceCenter.AssetType.scene, null);

        _OnSceneInit();

        mloader.CompletedCallBack =
            () =>
            {
                Done = true;

                mSceneBgm = FMOD_StudioSystem.instance.GetEvent(tb.bgm);
                if (mSceneBgm != null)
                    mSceneBgm.start();

                if (Application.platform == RuntimePlatform.WindowsEditor
                    || Application.platform == RuntimePlatform.OSXEditor)
                {
                    #region editor platform code
                    GameObject[] allgos = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

                    foreach (GameObject gg in allgos)
                    {
                        Renderer[] allrds = gg.GetComponentsInChildren<Renderer>();
                        foreach (Renderer r in allrds)
                        {
                            foreach (Material mat in r.sharedMaterials)
                            {
                                if (mat != null && mat.shader != null)
                                {
                                    string sname = mat.shader.name;
                                    //if (!mShaderDic.ContainsKey(sname))
                                    //{
                                    //    Debug.Log(sname + "\n");
                                    //    mShaderDic.Add(sname, sname);
                                    //}
                                    mat.shader = Shader.Find(sname);
                                }
                            }
                        }
                    }

                    if (RenderSettings.skybox != null)
                    {
                        RenderSettings.skybox.shader = Shader.Find(RenderSettings.skybox.shader.name);
                    }
                    #endregion
                }

                _OnSceneWasLoaded();

                if (OnLoadedCallback != null)
                    OnLoadedCallback();
            };
    }

    public void OnUpdate(float deltaTime)
    {
        mloader.Update();

        _OnUpdate(deltaTime);
    }

    public void Clear()
    {
        _OnClear();
        if (SceneBgm != null)
        {
            SceneBgm.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            SceneBgm.release();
        }
    }
}