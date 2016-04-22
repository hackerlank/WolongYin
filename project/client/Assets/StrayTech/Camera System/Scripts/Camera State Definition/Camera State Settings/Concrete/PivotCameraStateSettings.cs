using UnityEngine;
using System.Collections;
using System;

namespace StrayTech
{
    /// <summary>
    /// The args needed to control the Pivot Camera. 
    /// </summary>
    [Serializable]
    public class PivotCameraStateSettings : ICameraStateSettings
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The transform to pivot on.")]
            private Transform _pivotHost = null;
            [SerializeField]
            [Tooltip("The offset from the pivot host position.")]
            private Vector3 _pivotHostOffset = new Vector3(0.0f, 0.0f, 0.1f);
            [SerializeField]
            [Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
            private bool _useCameraCollision = false;
        #endregion inspector members

        #region properties
            public Transform PivotHost { get { return this._pivotHost; } }
            public Vector3 PivotHostOffset { get { return this._pivotHostOffset; } }
            public bool UseCameraCollision { get { return this._useCameraCollision; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Pivot; } }
        #endregion properties

        #region constructors
            public PivotCameraStateSettings(Transform pivotHost, Vector3 pivotHostOffset)
            {
                this._pivotHost = pivotHost;
                this._pivotHostOffset = pivotHostOffset;
            }
        #endregion constructors
    }
}