using UnityEngine;
using System.Collections;
using System;

namespace StrayTech
{
    /// <summary>
    /// The args needed to control the Isometric Camera. 
    /// </summary>
    [Serializable]
    public class IsometricCameraStateSettings : ICameraStateSettings
    {
        #region inspector members
            [SerializeField]
            [Tooltip("World space Euler rotation to lock the camera’s view to.")]
            private Vector3 _rotation = new Vector3(45, 90, 0);
            [SerializeField]
            [Tooltip("The distance the camera will be from the target.")]
            private float _distance = 5.0f;
            [SerializeField]
            [Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
            private bool _useCameraCollision = false;
        #endregion inspector members

        #region properties
            public Vector3 Rotation { get { return this._rotation; } }
            public float Distance { get { return this._distance; } }
            public bool UseCameraCollision { get { return this._useCameraCollision; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Isometric; } }
        #endregion properties

        #region constructors
            public IsometricCameraStateSettings(Vector3 rotation, float distance)
            {
                this._rotation = rotation;
                this._distance = distance;
            }
        #endregion constructors
    }
}