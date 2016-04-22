using UnityEngine;
using System.Collections.Generic;


public class LoadingWindow : SingletonWindow<LoadingWindow>
{
    UISlider mslider = null;
    float mNextValue = 0f;
    float mSpeed = 3f;
    bool mSmooth = true;
    public bool Smooth
    {
        get { return mSmooth; }
        set { mSmooth = value; }
    }

    protected override void OnInit()
    {
        mslider = GetComponent<UISlider>("ProgressBar");
    }

    public float Progress
    {
        set
        {
            if (Smooth)
            {
                mNextValue = value;
            }
            else
            {
                _SetSliderValue(value);
            }
        }
    }

    void _SetSliderValue(float val)
    {
        if (mslider != null) mslider.value = val;
    }

    protected override void OnOpen(params object[] Parameters)
    {
        base.OnOpen(Parameters);
        mNextValue = 0f;
        _SetSliderValue(0);
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (mSmooth)
        {
            if (mslider != null)
            {
                float cur = Mathf.Lerp(mslider.value, mNextValue, mSpeed * Time.unscaledDeltaTime);
                _SetSliderValue(cur);
            }
        }
    }
}
