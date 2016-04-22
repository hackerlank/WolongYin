using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// Camera that pivots on a fixed point and looks at the target. 
    /// </summary>
    public class PivotCamera : ICameraState
    {
        #region members
            private PivotCameraStateSettings _stateSettings;
        #endregion members

        #region properties
            public ICameraStateSettings StateSettings { get { return _stateSettings; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Pivot; } }
            public bool AllowsModifiers { get { return true; } }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
        #endregion properties

        #region constructors
            public PivotCamera(ICameraStateSettings stateSettings)
            {
                this._stateSettings = stateSettings as PivotCameraStateSettings;
            }
        #endregion construcors

        #region methods
            public void UpdateCamera(float deltaTime)
            {
                if (CameraSystem.Instance.CameraTarget == null)
                {
                    return;
                }

                Vector3 directionVector = (CameraSystem.Instance.CameraTarget.position - this._stateSettings.PivotHost.position);
                Vector3 flattenedDirection = new Vector3(directionVector.x, 0, directionVector.z).normalized;

                Vector3 cameraPosition = this._stateSettings.PivotHost.position + (Quaternion.LookRotation(flattenedDirection, Vector3.up) * this._stateSettings.PivotHostOffset);
                this.Position = cameraPosition;
                this.Rotation = Quaternion.LookRotation((CameraSystem.Instance.CameraTarget.position - this.Position).normalized, Vector3.up);
            }

            public void Cleanup()
            {

            }
        #endregion methods
    }
}