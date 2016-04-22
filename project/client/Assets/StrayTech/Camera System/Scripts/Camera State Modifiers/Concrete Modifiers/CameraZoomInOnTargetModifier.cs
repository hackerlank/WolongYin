using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StrayTech
{
    public class CameraZoomInOnTargetModifier : CameraStateModifierBase
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The target to zoom in on.")]
            private Transform _target;

            [SerializeField]
            [Tooltip("The offset from target.")]
            private Vector3 _targetOffset;

            [SerializeField]
            [CustomAttributes.NonNegative]
            [Tooltip("The distance to zoom into from target.")]
            private float _distanceFromTarget = 8.0f;
        #endregion inspector members

        #region properties
            public override string Name
            {
                get { return "Camera Zoom In On Target Modifier"; }
            }
        #endregion properties

        #region methods
            protected override void CalculateModification(ICameraState cameraState, float deltaTime)
            {
                Vector3 modifiedTarget = this._target.position + this._targetOffset;
                Vector3 targetToCameraNormal = (cameraState.Position - modifiedTarget).normalized;

                this._cameraTargetPosition = modifiedTarget + (targetToCameraNormal * this._distanceFromTarget);
                this._cameraTargetRotation = Quaternion.LookRotation(-targetToCameraNormal, Vector3.up);
            }
        #endregion methods
    }
}