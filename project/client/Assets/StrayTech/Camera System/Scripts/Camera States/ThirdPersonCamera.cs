using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// Basic third person camera. 
    /// </summary>
    public class ThirdPersonCamera : ICameraState
    {
        #region members
            private ThirdPersonCameraStateSettings _stateSettings;
            private Transform _cameraLookAtTransform = null;

            private float _orbitDistance = 1.0f;
            private float _mouseOrbitY = 0;
            private float _mouseOrbitX = 0;
            private Quaternion _currentOrbitRotation;
        #endregion members

        #region properties
            public ICameraStateSettings StateSettings { get { return _stateSettings; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.ThirdPerson; } }
            public bool AllowsModifiers { get { return true; } }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        #endregion properties

        #region constructors
            public ThirdPersonCamera(ICameraStateSettings stateSettings)
            {
                this._stateSettings = stateSettings as ThirdPersonCameraStateSettings;

                this._cameraLookAtTransform = CameraSystem.Instance.CameraTarget;
                this._orbitDistance = (this._stateSettings.MouseOrbitDistance.x + this._stateSettings.MouseOrbitDistance.y) * 0.5f;

                if (this._stateSettings.MouseOrbit == true)
                {
                    Vector3 dirCameraToTarget = (CameraSystem.Instance.CameraTarget.position - CameraSystem.Instance.CurrentCamera.transform.position).normalized;
                    Quaternion initialOrbitRotation = Quaternion.LookRotation(dirCameraToTarget, Vector3.up);
                    this._mouseOrbitX = initialOrbitRotation.eulerAngles.x;
                    this._mouseOrbitY = initialOrbitRotation.eulerAngles.y;
                }

                UpdateCamera(100.0f);
            }
        #endregion construcors

        #region methods
            public void UpdateCamera(float deltaTime)
            {
                if (this._cameraLookAtTransform == null)
                {
                    return;
                }

                if (this._stateSettings.MouseOrbit == true)
                {
                    this._mouseOrbitY += Input.GetAxis("Mouse X") * this._stateSettings.MouseSensitivity.x * this._orbitDistance * 0.02f;
                    this._mouseOrbitX -= Input.GetAxis("Mouse Y") * this._stateSettings.MouseSensitivity.y * 0.02f * (this._stateSettings.MouseInvertY ? -1 : 1);

                    this._mouseOrbitX = ClampAngle(this._mouseOrbitX, this._stateSettings.MousePitchRange.x, this._stateSettings.MousePitchRange.y);

                    Quaternion rotation = Quaternion.Euler(this._mouseOrbitX, this._mouseOrbitY, 0);

                    this._currentOrbitRotation = Quaternion.Slerp(this._currentOrbitRotation, rotation, this._stateSettings.MotionSmoothing * deltaTime);

                    this._orbitDistance = Mathf.Clamp(this._orbitDistance - Input.GetAxis("Mouse ScrollWheel") * 5, this._stateSettings.MouseOrbitDistance.x, this._stateSettings.MouseOrbitDistance.y);

                    Vector3 negDistance = new Vector3(0.0f, 0.0f, -this._orbitDistance);
                    Vector3 position = this._currentOrbitRotation * negDistance + this._cameraLookAtTransform.position;

                    this.Rotation = this._currentOrbitRotation;
                    this.Position = position;
                }
                else
                {
                    Vector3 cameraPosition = this._cameraLookAtTransform.position + (this._cameraLookAtTransform.rotation * this._stateSettings.TargetOffset);

                    Vector3 cameraPositionTarget = Vector3.Lerp(this.Position, cameraPosition, this._stateSettings.MotionSmoothing * deltaTime);

                    float cameraDistance = (cameraPositionTarget - this._cameraLookAtTransform.position).magnitude;
                    Vector3 cameraReverseDirection = (cameraPositionTarget - this._cameraLookAtTransform.position).normalized;

                    Quaternion targetRotation = Quaternion.LookRotation(this._cameraLookAtTransform.position - this.Position, Vector3.up);
                    this.Position = this._cameraLookAtTransform.position + (cameraReverseDirection * cameraDistance);
                    this.Rotation = targetRotation;
                }
            }

            public void Cleanup() {}

            public static float ClampAngle(float angle, float min, float max)
            {
                if (angle < -360F)
                    angle += 360F;
                if (angle > 360F)
                    angle -= 360F;
                return Mathf.Clamp(angle, min, max);
            }
        #endregion methods
    }
}