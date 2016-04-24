using UnityEngine;
using System.Collections.Generic;


public class GameMonoAgent : MonoBehaviour
{
    private List<BaseGameMono> mMonoList = new List<BaseGameMono> ();

    public T AddGameMonoComponent<T>() where T : BaseGameMono
    {
        T mono = GetGameMonoComponent<T>();
        if (mono == null)
        {
            mono = System.Activator.CreateInstance<T>();
            mono.started = false;
            mono.attachedMono = this;
            mono.gameObject = gameObject;
            mono.transform = gameObject.transform;
            mono.name = gameObject.name;
            mono.Awake();
            mMonoList.Add(mono);
        }
        return mono;
    }

    public T GetGameMonoComponent<T>() where T : BaseGameMono
    {
        BaseGameMono mono = mMonoList.Find((BaseGameMono gm) => gm.GetType() == typeof(T));
        if (mono != null) return (T)mono;
        else return null;
    }

    #region mono funcs
    void Awake() { }

    void Start() { }

    void Update()
    {
        for (int i=0; i<mMonoList.Count; ++i)
        {
            BaseGameMono mono = mMonoList[i];
            if (!mono.enable)
                continue;

            if (!mono.started)
            {
                mono.Start();
                mono.started = true;
            }

            mono.Update(mono.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < mMonoList.Count; ++i)
        {
            BaseGameMono mono = mMonoList[i];
            if (!mono.enable)
                continue;

            mono.LateUpdate();
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < mMonoList.Count; ++i)
        {
            BaseGameMono mono = mMonoList[i];
            if (!mono.enable)
                continue;

            mono.FixedUpdate();
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < mMonoList.Count; ++i)
        {
            BaseGameMono mono = mMonoList[i];
            mono.OnDestroy();
        }

        mMonoList.Clear();
    }

    void OnGUI()
    {
        for (int i = 0; i < mMonoList.Count; ++i)
        {
            BaseGameMono mono = mMonoList[i];
            if (!mono.enable)
                continue;

            mono.OnGUI();
        }
    }
    #endregion

} 