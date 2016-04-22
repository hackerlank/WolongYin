using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public class CameraStateModifierTimedTrigger : CameraSystemTriggerBase
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The target Camera Modifier to enable and/or disable.")]
            private CameraStateModifierBase _cameraStateModifierTarget = null;

            [SerializeField]
            [Tooltip("The modifier will be disabled after this duration.")]
            private float _enabledDuration = 1.0f;
        #endregion inspector members

        #region monobehaviour callbacks
            protected override void TriggerEntered()
            {
                this._cameraStateModifierTarget.Enable();
                StartCoroutine(DoTimedDisable());
            }

            protected override void TriggerExited()
            {
                // Do nothing
            }

            private IEnumerator DoTimedDisable()
            {
                yield return new WaitForSeconds(this._enabledDuration);

                this._cameraStateModifierTarget.Disable();

                if (this._singleUseTrigger == true)
                {
                    this.gameObject.SetActive(false);
                }
            }
        #endregion monobehaviour callbacks
    }
}
