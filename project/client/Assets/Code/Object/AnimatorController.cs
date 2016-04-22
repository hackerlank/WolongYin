using UnityEngine;
using System.Collections.Generic;


public class AnimatorController : BaseGameMono, IActionControllerPlayable
{

    private bool mIgnoreTimeScale = false;
    private Animator mAnimator = null;
    private EActionState mState = EActionState.stop;

    public EActionState actionState
    {
        get { return mState; }
        private set { mState = value; }
    }

    public UnityEngine.Animator animator
    {
        get { return mAnimator; }
    }

    public bool IgnoreTimeScale
    {
        get { return mIgnoreTimeScale; }
        set
        {
            mIgnoreTimeScale = value;

            if (IgnoreTimeScale)
            {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else
            {
                animator.updateMode = AnimatorUpdateMode.Normal;
            }
        }
    }

    public override void Awake()
    {
        mAnimator = gameObject.GetComponent<Animator>();
    }

    public void CrossFade(string name, float blendtime = 0.3f, float normalizedTime = 0f)
    {
        if (animator == null)
            return;

        actionState = EActionState.playing;
    }

    public void Pause()
    {
        if (animator == null)
            return;

        actionState = EActionState.pause;
        animator.speed = 0f;
    }

    public void Resume()
    {
        if (animator == null)
            return;

        actionState = EActionState.playing;
        animator.speed = 1f;
    }

    public void Stop()
    {
        if (animator == null)
            return;

        actionState = EActionState.stop;
        animator.Stop();
    }
}