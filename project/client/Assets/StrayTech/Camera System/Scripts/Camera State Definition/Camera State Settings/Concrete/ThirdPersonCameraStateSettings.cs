using UnityEngine;
using System.Collections;
using System;

namespace StrayTech
{
    /// <summary>
    /// The args needed to control the Third Person Camera. 
    /// </summary>
    [Serializable]
    public class ThirdPersonCameraStateSettings : ICameraStateSettings
    {
        #region inspector members
            [SerializeField]
            [Tooltip("Use the mouse to control the camera's orbit.")]
            private bool _mouseOrbit = false;
            [SerializeField]
            [Tooltip("The position offset from the target.")]
            private Vector3 _targetOffset = new Vector3(0.0f, 6.0f, -5.0f);
            [SerializeField]
            [Tooltip("The minimum and maximum distance the camera can be from the target.")]
            private Vector2 _mouseOrbitDistance = new Vector2(1.0f, 5.0f);
            [SerializeField]
            [Tooltip("The range of vertical rotation.")]
            private Vector2 _mousePitchRange = new Vector2(-90.0f, 90.0f);
            [SerializeField]
            [Tooltip("Sensitivity of mouse movement on each axis.")]
            private Vector2 _mouseSensitivity = new Vector2(2.0f, 2.0f);
            [SerializeField]
            [Tooltip("Invert mouse Y axis?")]
            private bool _mouseInvertY = false;
            [SerializeField]
            [CustomAttributes.NonNegative]
            [Tooltip("The amount of smoothing to apply.")]
            private float _motionSmoothing = 6.0f;
            [SerializeField]
            [Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
            private bool _useCameraCollision = false;
        #endregion inspector members

        #region properties
            public bool MouseOrbit { get { return this._mouseOrbit; } }
            public Vector3 TargetOffset { get { return this._targetOffset; } }
            public Vector2 MouseOrbitDistance { get { return this._mouseOrbitDistance; } }
            public Vector2 MousePitchRange { get { return this._mousePitchRange; } }
            public Vector2 MouseSensitivity { get { return this._mouseSensitivity; } }
            public bool MouseInvertY { get { return this._mouseInvertY; } }
            public float MotionSmoothing { get { return this._motionSmoothing; } }
            public bool UseCameraCollision { get { return this._useCameraCollision; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.ThirdPerson; } }
        #endregion properties

        #region constructors
            public ThirdPersonCameraStateSettings(Vector3 targetOffset, bool mouseOrbit, Vector2 mouseOrbitDistance, Vector2 mousePitchRange, Vector2 mouseSensitivity, bool mouseInvertY, float motionSmoothing)
            {
                this._targetOffset = targetOffset;
                this._mouseOrbit = mouseOrbit;
                this._mouseOrbitDistance = mouseOrbitDistance;
                this._mousePitchRange = mousePitchRange;
                this._mouseSensitivity = mouseSensitivity;
                this._mouseInvertY = mouseInvertY;
                this._motionSmoothing = motionSmoothing;
            }
        #endregion constructors
    }
}