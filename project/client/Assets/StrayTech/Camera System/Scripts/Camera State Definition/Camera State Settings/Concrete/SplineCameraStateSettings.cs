using UnityEngine;
using System.Collections;
using System;

namespace StrayTech
{
    /// <summary>
    /// The args needed to control the Spline Camera. 
    /// </summary>
    [Serializable]
    public class SplineCameraStateSettings : ICameraStateSettings
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The spline to use.")]
            private BezierSpline _spline = null;
            [SerializeField]
            [Tooltip("Offset the camera on the spline from the start in world units of length.")]
            private float _splinePositionOffset = 0.0f;
            [SerializeField]
            [Tooltip("Offset along the line of sight to the target.")]
            private float _cameraLineOfSightOffset = 0;
            [SerializeField]
            [Tooltip("Maximum distance the camera can be from the target.")]
            private float _cameraMaxDistance = 5.0f;
            [SerializeField]
            [Tooltip("The maximum speed the camera can travel along the spline in world units of length per second.")]
            private float _splineTravelMaxSpeed = 0.1f;
            [SerializeField]
            [Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
            private bool _useCameraCollision = false;
        #endregion inspector members

        #region properties
            public BezierSpline Spline { get { return this._spline; } }
            public float SplinePositionOffset { get { return this._splinePositionOffset; } }
            public float CameraLineOfSightOffset { get { return this._cameraLineOfSightOffset; } }
            public float CameraMaxDistance { get { return this._cameraMaxDistance; } }
            public float SplineTravelMaxSpeed { get { return this._splineTravelMaxSpeed; } }
            public bool UseCameraCollision { get { return this._useCameraCollision; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Spline; } }
        #endregion properties

        #region constructors
            public SplineCameraStateSettings(BezierSpline spline, float splinePositionOffset, float cameraLineOfSightOffset, float cameraMaxDistance, float splineTravelMaxSpeed)
            {
                this._spline = spline;
                this._splinePositionOffset = splinePositionOffset;
                this._cameraLineOfSightOffset = cameraLineOfSightOffset;
                this._cameraMaxDistance = cameraMaxDistance;
                this._splineTravelMaxSpeed = splineTravelMaxSpeed;
            }
        #endregion constructors
    }
}