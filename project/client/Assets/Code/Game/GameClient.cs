using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;


public class GameClient
{
    private float m_curTimeScaleDuration = 0f;
    private bool m_bScalingTime = false;
    private Utility.VoidDelegate m_TimeScaleCallback = null;

    public Utility.VoidDelegate TimeScaleCallback
    {
        get { return m_TimeScaleCallback; }
        set { m_TimeScaleCallback = value; }
    }

    public float CurTimeScaleDuration
    {
        get { return m_curTimeScaleDuration; }
    }

    public void OnAwake()
    {
    }

    public void OnStart()
    {
        _SetupResourceCenter();

        ClockMgr.instance.Initialize();
        PoolManager.instance.Initialize();
        StageManager.instance.Initialize();
        WindowManager.Build();

        StageManager.instance.SetActiveState((int)GameDef.EGameStage.startup_stage);
    }

    public void InitClientFiles()
    {
        _LoadClientConfigure("");
    }

    public void OnUpdate(float deltaTime)
    {
        ClockMgr.instance.Update();

        StageManager.instance.OnUpdate(deltaTime);

        _UpdateScalingTime();

        _DebugInput();

    }

    public void OnLateUpdate()
    { }

    public void OnFixedUpdate()
    { }

    public void OnDestroy()
    {
    }

    public void OnGUI()
    {
        if (StageManager.instance.ActiveState != null)
            StageManager.instance.ActiveState.OnGUI();
    }


    public void ScaleTime(float scale, float duration, Utility.VoidDelegate callback)
    {
        Time.timeScale = scale;
        m_bScalingTime = true;
        m_curTimeScaleDuration = duration;
        m_TimeScaleCallback = callback;

        //Tween<float> tf = new Tween<float>();
        //tf.SetDelay(duration);
        //tf.SetStartValue(0);
        //tf.SetEndValue(1);
        //tf.SetTimeScaleIndependent(true);
        //tf.OnCompleted(
        //    (value) =>
        //    {
        //        Time.timeScale = 1.0f;
        //        if (callback != null)
        //            callback();
        //    });
        //tf.Play();
    }

    public void ResetTimeScale()
    {
        m_bScalingTime = false;
        Time.timeScale = 1.0f;
    }

    void _UpdateScalingTime()
    {
        if (!m_bScalingTime)
            return;

        m_curTimeScaleDuration -= ClockMgr.instance.RealDeltaTime;

        if (m_curTimeScaleDuration <= 0)
        {
            ResetTimeScale();
            if (TimeScaleCallback != null)
            {
                TimeScaleCallback();
                m_TimeScaleCallback = null;
            }
        }
    }

    void _LoadClientConfigure(string path)
    {
        _LoadTable(path + "table");
        _LoadXml(path + "xml");
    }


    void _LoadTable(string folder)
    {
//         if (!RoleBaseManager.instance.Load(folder))
//         {
//             Logger.instance.Error("load {0} failed!\n", RoleBaseManager.instance.source);
//         }
    }

    void _LoadXml(string folder)
    {
//         if (!GameSetupXmlClass.instance.Load(folder))
//         {
//             Logger.instance.Error("load {0} failed!\n", GameSetupXmlClass.instance.FileName);
//         }
    }

    void _SetupResourceCenter()
    {
        ResourceCenter.Build(ClientSetup.Get().listWorkingPaths);
        //ResourceCenter.USE_CLEAR_SCENE = true;
    }




    void _DebugInput()
    {
    }
}
