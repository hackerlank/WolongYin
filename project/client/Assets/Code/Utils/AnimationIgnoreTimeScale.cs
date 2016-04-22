using UnityEngine;
using System.Collections.Generic;


public class AnimationIgnoreTimeScale : MonoBehaviour
{
    private string mClipName = string.Empty;
    private bool mPlaying = false;
    private Animation mAnimation = null;
    private float mProgressTime = 0f;


    public void Play(string clipName, Animation anim)
    {
        mClipName = clipName;
        mPlaying = true;
        mProgressTime = 0f;
        mAnimation = anim;
        if (mAnimation == null)
            mAnimation = gameObject.GetComponent<Animation>();
    }

    public void Stop()
    {
        mPlaying = false;
    }
    
    void  Update()
    {
        if (!mPlaying || mAnimation == null)
            return;

        AnimationState animState = mAnimation[mClipName];

        if (animState == null)
            return;

        mProgressTime += ClockMgr.instance.RealDeltaTime;
        animState.normalizedTime = mProgressTime / animState.length;
        mAnimation.Sample(); 
    }
}