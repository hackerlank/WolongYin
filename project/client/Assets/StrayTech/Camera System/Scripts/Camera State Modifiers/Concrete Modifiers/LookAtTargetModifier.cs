using UnityEngine;
using System.Collections;

namespace StrayTech
{
    /// <summary>
    /// Camera modifier that causes the camera to look at an arbitrary transform when enabled.
    /// </summary>
    public class LookAtTargetModifier : CameraStateModifierBase
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The target Transform to look at.")]
            private Transform _lookAtTarget;
        #endregion inspector members

        #region properties
            public override string Name
            {
                get { return "Look At Target Modifier"; }
            }
        #endregion properties

        #region methods
            protected override void CalculateModification(ICameraState cameraState, float deltaTime)
            {
                if (this._lookAtTarget != null)
                {
                    this._cameraTargetPosition = cameraState.Position;
                    this._cameraTargetRotation = Quaternion.LookRotation((this._lookAtTarget.position - cameraState.Position).normalized, Vector3.up);
                }
            }
        #endregion methods
    }
}