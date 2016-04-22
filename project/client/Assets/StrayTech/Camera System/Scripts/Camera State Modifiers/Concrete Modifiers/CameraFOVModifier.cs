using System;
using StrayTech.CustomAttributes;
using UnityEngine;

namespace StrayTech
{
    /// <summary>
    /// A camera modifier that interpolates the main camera's field of view to a target value while it is enabled.
    /// </summary>
    public class CameraFOVModifier : CameraStateModifierBase
    {
        #region inspector members
            [CustomAttributes.NonNegative]
            [SerializeField]
            [Tooltip("The Field of View that this modifier adjusts the camera to when it's active.")]
            private float _fieldOfView = 60;
        #endregion inspector members

        #region members
            /// <summary>
            /// The FoV of the camera before this modfier was enabled. Cached so we can restore it when we are disabled.
            /// </summary>
            private float _cachedFoV;
        #endregion members

        #region properties
            public override string Name
            {
                get { return "Camera FOV Modifier"; }
            }
        #endregion properties

        #region methods
            protected override void CalculateModification(ICameraState cameraState, float deltaTime)
            {
                this._cameraTargetPosition = cameraState.Position;
                this._cameraTargetRotation = cameraState.Rotation;

                
                CameraSystem.Instance.ChangeCameraFOV(Mathf.Lerp(this._cachedFoV, this._fieldOfView, TransitionLerpT));
            }

            public override bool Enable()
            {
                this._cachedFoV = CameraSystem.Instance.CurrentCamera.fieldOfView;
                return base.Enable();
            }

            public override void Cleanup()
            {
                CameraSystem.Instance.ChangeCameraFOV(this._cachedFoV);
            }
        #endregion methods
    }
}