using UnityEngine;
using System.Collections;

namespace StrayTech
{
    /// <summary>
    /// A spline following camera.
    /// </summary>
    public class SplineCamera : ICameraState
    {
        #region members
            private SplineCameraStateSettings _stateSettings;
            private Transform _cameraLookAtTransform = null;
            private float _currentSplineT = -1.0f;
        #endregion members

        #region properties
            public ICameraStateSettings StateSettings { get { return _stateSettings; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Spline; } }
            public bool AllowsModifiers { get { return true; } }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        #endregion properties

        #region constructors
            public SplineCamera(ICameraStateSettings stateSettings)
            {
                this._stateSettings = stateSettings as SplineCameraStateSettings;

                this._currentSplineT = -1.0f;
            }
        #endregion construcors

        #region methods
            public void UpdateCamera(float deltaTime)
            {
                if (this._cameraLookAtTransform == null)
                {
                    this._cameraLookAtTransform = CameraSystem.Instance.CameraTarget;
                    if (this._cameraLookAtTransform == null)
                    {
                        return;
                    }
                }

                if (this._stateSettings.Spline == null) return;

                float sampledSplineT = this._stateSettings.Spline.GetClosestPointParam(this._cameraLookAtTransform.position, 5);
                
                if (this._stateSettings.Spline.Loop == true)
                {
                    sampledSplineT = sampledSplineT + 0.5f;

                    sampledSplineT = Mathf.Abs(sampledSplineT % 1.0f);

                    if (this._currentSplineT < 0)
                    {
                        this._currentSplineT = sampledSplineT;
                    }

                    Vector3 centerToPlayerVector = this._cameraLookAtTransform.position - this._stateSettings.Spline.transform.position;
                    centerToPlayerVector.y = 0;
                    float centerToPlayerDistance = centerToPlayerVector.magnitude;
                    float centerToEdge = Mathf.Max(this._stateSettings.Spline.transform.lossyScale.x, this._stateSettings.Spline.transform.lossyScale.z);
                    float distanceNormalized = centerToPlayerDistance / centerToEdge;

                    float speed = this._stateSettings.SplineTravelMaxSpeed;
                    speed *= Mathf.Clamp01(Mathf.Pow(distanceNormalized, 4));

                    Quaternion start = Quaternion.Euler(0, this._currentSplineT * 360.0f, 0);
                    Quaternion end = Quaternion.Euler(0, sampledSplineT * 360.0f, 0);
                    Quaternion lerpResult = Quaternion.Slerp(start, end, speed * deltaTime);
                    this._currentSplineT = lerpResult.eulerAngles.y / 360.0f;
                    this._currentSplineT = Mathf.Abs(this._currentSplineT % 1.0f);
                }
                else
                {
                    if (this._currentSplineT < 0)
                    {
                        this._currentSplineT = sampledSplineT;
                    }

                    float splinePositionOffsetT = this._stateSettings.SplinePositionOffset / this._stateSettings.Spline.Length;
                    this._currentSplineT = Mathf.Lerp(this._currentSplineT, sampledSplineT + splinePositionOffsetT, this._stateSettings.SplineTravelMaxSpeed * deltaTime);
                }

                Vector3 positionOnSpline = this._stateSettings.Spline.GetPosition(this._currentSplineT);

                this.Rotation = Quaternion.LookRotation(this._cameraLookAtTransform.position - positionOnSpline, Vector3.up);
                this.Position = positionOnSpline + (this.Rotation * Vector3.forward * this._stateSettings.CameraLineOfSightOffset);

                float distance = Vector3.Distance(this.Position, this._cameraLookAtTransform.position);
                if (distance > this._stateSettings.CameraMaxDistance)
                {
                    this.Position += this.Rotation * (Vector3.forward * (distance - this._stateSettings.CameraMaxDistance));
                }
            }

            public void Cleanup()
            {
                this._currentSplineT = -1.0f;
            }
        #endregion methods
    }
}