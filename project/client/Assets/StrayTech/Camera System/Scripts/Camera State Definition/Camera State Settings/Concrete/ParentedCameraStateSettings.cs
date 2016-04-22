using UnityEngine;
using System.Collections;
using System;

namespace StrayTech
{
    /// <summary>
    /// The args needed to control the Parented Camera. 
    /// </summary>
    [Serializable]
    public class ParentedCameraStateSettings : ICameraStateSettings
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The GameObject to parent to.")]
            private GameObject _parent = null;
            [SerializeField]
            [Tooltip("Position offset from parent.")]
            private Vector3 _positionOffset = new Vector3(0.0f, 0.0f, 0.0f);
            [SerializeField]
            [Tooltip("Rotation offset from parent in euler angles.")]
            private Vector3 _rotationOffset = new Vector3(0.0f, 0.0f, 0.0f);
            [SerializeField]
            [Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
            private bool _useCameraCollision = false;
        #endregion inspector members

        #region properties
            public GameObject Parent { get { return this._parent; } }
            public Vector3 PositionOffset { get { return this._positionOffset; } }
            public Vector3 RotationOffset { get { return this._rotationOffset; } }
            public bool UseCameraCollision { get { return this._useCameraCollision; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.Parented; } }
        #endregion properties

        #region constructors
            public ParentedCameraStateSettings(GameObject parent, Vector3 positionOffset, Vector3 rotationOffset)
            {
                this._parent = parent;
                this._positionOffset = positionOffset;
                this._rotationOffset = rotationOffset;
            }
        #endregion constructors
    }
}