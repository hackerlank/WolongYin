using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// Camera that is parented to a transform. 
    /// </summary>
    public class ParentedCamera : ICameraState
    {
        #region members
            private ParentedCameraStateSettings _stateSettings;
        #endregion members

        #region properties
            public ICameraStateSettings StateSettings { get { return _stateSettings; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Parented; } }
            public bool AllowsModifiers { get { return true; } }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        #endregion properties

        #region constructors
            public ParentedCamera(ICameraStateSettings stateSettings)
            {
                this._stateSettings = stateSettings as ParentedCameraStateSettings;
            }
        #endregion construcors

        #region methods
            public void UpdateCamera(float deltaTime)
            {
                this.Position = this._stateSettings.Parent.transform.position + (this._stateSettings.Parent.transform.rotation * this._stateSettings.PositionOffset);
                this.Rotation = Quaternion.Euler(this._stateSettings.RotationOffset) * this._stateSettings.Parent.transform.rotation;
            }

            public void Cleanup()
            {

            }
        #endregion methods
    }
}