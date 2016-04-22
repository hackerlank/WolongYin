using UnityEngine;
using System.Collections.Generic;


public class ClockMgr : Singleton<ClockMgr>
{
    private float mRealLastTime;
    private float mRealDeltaTime;

    public float RealDeltaTime
    {
        get { return mRealDeltaTime; }
    }
    public float RealLastTime
    {
        get { return mRealLastTime; }
    }

    public void Initialize()
    {
        mRealLastTime = Time.realtimeSinceStartup;
    }


    public void Update()
    {
        float t = Time.realtimeSinceStartup;
        mRealDeltaTime = t - mRealLastTime;
        mRealLastTime = t;
    }
}