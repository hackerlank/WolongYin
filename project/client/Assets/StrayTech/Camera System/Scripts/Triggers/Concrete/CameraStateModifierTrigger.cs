using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public class CameraStateModifierTrigger : CameraSystemTriggerBase
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The target Camera Modifier to enable and/or disable.")]
            private CameraStateModifierBase _cameraStateModifierTarget = null;

            [SerializeField]
            [Tooltip("Ignore the OnTriggerExit event?")]
            private bool _ignoreTriggerExit = false;
        #endregion inspector members

        #region monobehaviour callbacks
            protected override void TriggerEntered()
            {
                this._cameraStateModifierTarget.Enable();
            }

            protected override void TriggerExited()
            {
                if (this._ignoreTriggerExit == true)
                {
                    return;
                }

                this._cameraStateModifierTarget.Disable();
            }
        #endregion monobehaviour callbacks
    }
}
