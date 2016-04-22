using UnityEngine;
using System.Collections;

namespace StrayTech
{
    /// <summary>
    /// Basic first person perspective camera.
    /// </summary>
    public class FirstPersonCamera : ICameraState
    {
        #region members
            private FirstPersonCameraStateSettings _stateSettings;

            private Quaternion _characterTargetRot;
            private Quaternion _cameraTargetRot;
            private Quaternion _cameraPitchRotation;
        #endregion members

        #region properties
            public ICameraStateSettings StateSettings { get { return _stateSettings; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.FirstPerson; } }
            public bool AllowsModifiers { get { return true; } }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        #endregion properties

        #region constructors
            public FirstPersonCamera(ICameraStateSettings stateSettings)
            {
                this._stateSettings = stateSettings as FirstPersonCameraStateSettings;

                if (this._stateSettings.CharacterTransform != null)
                {
                    this._characterTargetRot = this._stateSettings.CharacterTransform.rotation;
                    this.Rotation = this._stateSettings.CharacterTransform.rotation;
                }

                this._cameraPitchRotation = Quaternion.identity;
            }
        #endregion construcors

        #region methods
            public void UpdateCamera(float deltaTime)
            {
                if (this._stateSettings.PositionRootTransform == null) return;
                if (this._stateSettings.CharacterTransform == null) return;

                Vector3 hostPosition = this._stateSettings.PositionRootTransform.position + (this._stateSettings.CharacterTransform.rotation * this._stateSettings.PositionOffset);
                this.Position = hostPosition;

                float yRotation = Input.GetAxis("Mouse X") * this._stateSettings.MouseLookSensitivity.x;
                float xRotation = Input.GetAxis("Mouse Y") * this._stateSettings.MouseLookSensitivity.y;

                this._characterTargetRot *= Quaternion.Euler(0f, yRotation, 0f);
                this._cameraPitchRotation *= Quaternion.Euler(-xRotation, 0f, 0f);

                this._cameraPitchRotation = ClampRotationAroundXAxis(this._cameraPitchRotation);
                this._cameraTargetRot = this._stateSettings.CharacterTransform.rotation * this._cameraPitchRotation;

                this._stateSettings.CharacterTransform.rotation = Quaternion.Slerp(this._stateSettings.CharacterTransform.rotation, this._characterTargetRot, this._stateSettings.MouseSmoothing * deltaTime);
                this.Rotation = Quaternion.Slerp(this.Rotation, this._cameraTargetRot, this._stateSettings.MouseSmoothing * deltaTime);
            }

            Quaternion ClampRotationAroundXAxis(Quaternion q)
            {
                q.x /= q.w;
                q.y /= q.w;
                q.z /= q.w;
                q.w = 1.0f;

                float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

                angleX = Mathf.Clamp(angleX, this._stateSettings.PitchRange.x, this._stateSettings.PitchRange.y);

                q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

                return q;
            }

            public void Cleanup() {}
        #endregion methods
    }
}