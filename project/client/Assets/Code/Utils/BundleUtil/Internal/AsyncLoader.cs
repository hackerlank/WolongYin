using UnityEngine;
using System.Collections;


public class AsyncObjectLoader : UnityCoroutine
{
    private Object mObject = null;
    public UnityEngine.Object UObject
    {
        get { return mObject; }
    }

    public AsyncObjectLoader(string name)
    {
        routines.Enqueue(() =>
        {
            ResourceCenter.instance.LoadObject(name,
                (Object obj) =>
                {
                    mObject = obj;
                    isDone = true;
                });
        });
    }
}


public class AsyncSceneLoader : UnityCoroutine
{
    public AsyncSceneLoader(string name)
    {
        routines.Enqueue(() =>
        {
            ResourceCenter.instance.LoadScene(name,
                (Object obj) =>
                {
                    isDone = true;
                });
        });
    }
}


public class AsyncAudioLoader : UnityCoroutine
{
    AudioClip mClip = null;
    public AudioClip audio
    {
        get { return mClip; }
    }

    public AsyncAudioLoader(string name)
    {
        routines.Enqueue(() =>
        {
            ResourceCenter.instance.LoadAudio(name,
                (Object obj) =>
                {
                    mClip = obj as AudioClip;
                    isDone = true;
                });
        });
    }
}