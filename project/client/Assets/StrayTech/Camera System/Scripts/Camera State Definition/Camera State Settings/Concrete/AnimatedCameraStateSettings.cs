using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace StrayTech
{
    /// <summary>
    /// The args needed to control the CombatCameraState. 
    /// </summary>
    [Serializable]
    public class AnimatedCameraStateSettings : ICameraStateSettings
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The animation clip to play. (Needs to be a Legacy Animation Clip)")]
            private AnimationClip _animationClip;
            [SerializeField]
            [Tooltip("Use the parent override to override the root of the animation.")]
            private Transform _parentOverride;
            [SerializeField]
            [Tooltip("Y axis rotation adjustment (Some animations from Maya need adjustment)")]
            private float _yRotationFix;
        #endregion inspector members

        #region properties
            public AnimationClip AnimationClip { get { return this._animationClip; } }
            public Transform ParentOverride { get { return this._parentOverride; } }
            public float YRotationFix { get { return this._yRotationFix; } }
            public bool UseCameraCollision { get { return false; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Animated; } }
        #endregion properties

        #region constructors
            public AnimatedCameraStateSettings(AnimationClip animationClipToPlay, Transform parentOverride, float yRotationFix)
            {
                this._animationClip = animationClipToPlay;
                this._parentOverride = parentOverride;
                this._yRotationFix = yRotationFix;
            }
        #endregion constructors
    }
}