using UnityEngine;
using System.Collections;

namespace StrayTech
{
    /// <summary>
    /// An isometric locked perspective camera.
    /// </summary>
    public class IsometricCamera : ICameraState
    {
        #region members
            private IsometricCameraStateSettings _stateSettings;
        #endregion members

        #region properties
            public ICameraStateSettings StateSettings { get { return _stateSettings; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Isometric; } }
            public bool AllowsModifiers { get { return true; } }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        #endregion properties

        #region constructors
            public IsometricCamera(ICameraStateSettings stateSettings)
            {
                this._stateSettings = stateSettings as IsometricCameraStateSettings;
            }
        #endregion construcors

        #region methods
            public void UpdateCamera(float deltaTime)
            {
                if (CameraSystem.Instance.CameraTarget == null)
                {
                    return;
                }

                this.Position = CameraSystem.Instance.CameraTarget.position + ((Quaternion.Euler(this._stateSettings.Rotation) * -Vector3.forward) * this._stateSettings.Distance);
                this.Rotation = Quaternion.LookRotation(CameraSystem.Instance.CameraTarget.position - this.Position, Vector3.up);
            }

            public void Cleanup()
            {

            }
        #endregion methods
    }
}