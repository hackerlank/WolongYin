using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

namespace StrayTech
{
    public class CameraStateTransitionTrigger : CameraSystemTriggerBase
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The target Camera State Definition to transition to.")]
            private CameraStateDefinition _targetCameraStateDefinition;
        #endregion inspector members

        #region properties
            public CameraStateDefinition TargetCameraStateDefinition
            {
                get { return _targetCameraStateDefinition; }
                set { _targetCameraStateDefinition = value; }
            }
        #endregion properties

        #region monobehaviour callbacks
            protected override void TriggerEntered()
            {
                CameraSystem.Instance.RegisterCameraState(this._targetCameraStateDefinition);
            }

            protected override void TriggerExited()
            {
                CameraSystem.Instance.UnregisterCameraState(this._targetCameraStateDefinition);
            }
        #endregion monobehaviour callbacks
    }
}
