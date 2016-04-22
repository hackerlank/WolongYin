using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// A camera which is controlled by animation. 
    /// </summary>
    public class AnimatedCamera : ICameraState, IValidates
    {
        #region inner types
            public class OnFinishedEventArgs
            {
                // true if animation finished to the end, false if the camera system switched to a new state before animation finished.
                private bool _animationFinished = false;

                public bool AnimationFinished
                {
                    get { return _animationFinished; }
                }

                public OnFinishedEventArgs(bool animationFinished)
                {
                    this._animationFinished = animationFinished;
                }
            }
        #endregion inner types

        #region members
            private AnimatedCameraStateSettings _stateSettings;
            private float _clipDuration = 0.0f;
            private float _currentClipTime = 0.0f;
            private bool _animationComplete = false;
        #endregion members

        #region properties
            public ICameraStateSettings StateSettings { get { return _stateSettings; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Animated; } }
            public bool AllowsModifiers { get { return true; } }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        #endregion properties

        #region events
            /// <summary>
            /// Called when this has finished playing an animation. 
            /// </summary>
            public event Action<OnFinishedEventArgs> OnFinished;
            /// <summary>
            /// Called when thsi has started playing an animation. 
            /// </summary>
            public event Action OnStarted;
        #endregion events

        #region constructors
            public AnimatedCamera(ICameraStateSettings stateSettings)
            {
                this._stateSettings = stateSettings as AnimatedCameraStateSettings;

                //Bail if we not valid. 
                if (IsValid() == false)
                {
                    Debug.LogException(new FailedValidationException("AnimatedCamera"));
                    return;
                }

                this._clipDuration = this._stateSettings.AnimationClip.length;
                this._currentClipTime = 0.0f;

                //Notify listeners that the we have started playing a camera animation. 
                if (OnStarted != null)
                    OnStarted();
            }
        #endregion constructors

        #region methods
            /// <summary>
            /// Quit playing the current animation, if any. 
            /// </summary>
            public void StopCurrentAnimation()
            {
                if (IsValid() == false)
                    return;

                this._animationComplete = true;

                if (OnFinished != null)
                    this.OnFinished(new OnFinishedEventArgs(true));
            }

            /// <summary>
            /// Called by owner when it wants to update the camera. 
            /// </summary>
            /// <param name="deltaTime"></param>
            public void UpdateCamera(float deltaTime)
            {
                if (this._animationComplete == true)
                {
                    return;
                }

                if (IsValid() == false)
                {
                    this._animationComplete = true;

                    if (OnFinished != null)
                        this.OnFinished(new OnFinishedEventArgs(true));

                    return;
                }

                if (this._currentClipTime < this._clipDuration)
                {
                    this._currentClipTime += deltaTime;
                    if (this._currentClipTime > this._clipDuration)
                    {
                        this._currentClipTime = this._clipDuration;
                    }

                    GameObject cameraGO = CameraSystem.Instance.CurrentCamera.gameObject;
                    this._stateSettings.AnimationClip.SampleAnimation(cameraGO, this._currentClipTime);

                    if (this._stateSettings.ParentOverride != null)
                    {
                        this.Position = this._stateSettings.ParentOverride.transform.position + (this._stateSettings.ParentOverride.transform.rotation * cameraGO.transform.position);
                        this.Rotation = this._stateSettings.ParentOverride.transform.rotation * cameraGO.transform.rotation * Quaternion.Euler(0, this._stateSettings.YRotationFix, 0);
                    }
                    else
                    {
                        this.Position = cameraGO.transform.position;
                        this.Rotation = cameraGO.transform.rotation;
                        this.Rotation *= Quaternion.Euler(0, this._stateSettings.YRotationFix, 0);
                    }
                }
                else
                {
                    this._animationComplete = true;
                    Cleanup();
                    //Notify listeners we have finished playing the camera animation. 
                    if (OnFinished != null)
                        this.OnFinished(new OnFinishedEventArgs(true));
                }
            }

            /// <summary>
            /// Called when this state ends. 
            /// </summary>
            public void Cleanup()
            {
                //Bail if we not valid. 
                if (IsValid() == false)
                {
                    Debug.LogException(new FailedValidationException("AnimatedCamera"));
                    return;
                }

                if (this._animationComplete == false)
                {
                    if (OnFinished != null)
                        this.OnFinished(new OnFinishedEventArgs(false));
                }

                this._animationComplete = true;
            }

            /// <summary>
            /// Do we have all of our components? 
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                if (CameraSystem.Instance == null)
                    return false;

                if (CameraSystem.Instance.CurrentCamera == null)
                    return false;

                if (this._stateSettings == null)
                    return false;

                if (this._stateSettings.AnimationClip == null || string.IsNullOrEmpty(this._stateSettings.AnimationClip.name) == true)
                    return false;

                return true;
            }
        #endregion methods
    }
}