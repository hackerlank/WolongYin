using UnityEngine;
using System.Collections;
using System;

namespace StrayTech
{
    /// <summary>
    /// The args needed to control the FirstPersonCamera. 
    /// </summary>
    [Serializable]
    public class FirstPersonCameraStateSettings : ICameraStateSettings
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The transform that the camera’s position will be parented to.")]
            private Transform _positionRootTransform = null;
            [SerializeField]
            [Tooltip("The root transform of the character. (Y axis camera rotation is applied to the root)")]
            private Transform _characterTransform = null;
            [SerializeField]
            [Tooltip("Position offset from the PositionRootTransform")]
            private Vector3 _positionOffset = new Vector3(0.0f, 0.0f, 0.1f);
            [SerializeField]
            [Tooltip("The range of vertical rotation.")]
            private Vector2 _pitchRange = new Vector2(-90.0f, 90.0f);
            [SerializeField]
            [Tooltip("Sensitivity of mouse movement on each axis.")]
            private Vector2 _mouseLookSensitivity = new Vector2(2.0f, 2.0f);
            [SerializeField]
            [Tooltip("Amount of mouse smoothing to apply.")]
            private float _mouseSmoothing = 5.0f;
        #endregion inspector members

        #region properties
            public Transform PositionRootTransform { get { return this._positionRootTransform; } }
            public Transform CharacterTransform { get { return this._characterTransform; } }
            public Vector3 PositionOffset { get { return this._positionOffset; } }
            public Vector2 PitchRange { get { return this._pitchRange; } }
            public Vector2 MouseLookSensitivity { get { return this._mouseLookSensitivity; } }
            public float MouseSmoothing { get { return this._mouseSmoothing; } }
            public bool UseCameraCollision { get { return false; } }
            public CameraSystem.CameraStateEnum StateType { get { return CameraSystem.CameraStateEnum.FirstPerson; } }
        #endregion properties

        #region constructors
            public FirstPersonCameraStateSettings(Transform positionRoot, Transform characterTransform, Vector3 positionOffset, Vector2 pitchRange, Vector2 mouseLookSensitivity, float mouseSmoothing)
            {
                this._positionRootTransform = positionRoot;
                this._characterTransform = characterTransform;
                this._positionOffset = positionOffset;
                this._pitchRange = pitchRange;
                this._mouseLookSensitivity = mouseLookSensitivity;
                this._mouseSmoothing = mouseSmoothing;
            }
        #endregion constructors
    }
}