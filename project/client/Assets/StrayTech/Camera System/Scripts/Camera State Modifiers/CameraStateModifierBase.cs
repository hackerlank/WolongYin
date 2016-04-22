using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public abstract class CameraStateModifierBase : MonoBehaviour
    {
        #region inspector members
            [SerializeField]
            [Tooltip("Duration of the transition into the enabled state.")]
            private float _transitionIntoEnabledDuration = 1.0f;

            [SerializeField]
            [Tooltip("Duration of the transition into the disabled state.")]
            private float _transitionIntoDisabledDuration = 1.0f;

            [SerializeField]
            [Tooltip("The animation clip to play. (Needs to be a Legacy Animation Clip)")]
            private int _priority = int.MaxValue;
        #endregion inspector members

        #region members
            private bool _transitioning = false;
            private float _transitionLerpT = 0.0f;

            protected Vector3 _cameraTargetPosition;
            protected Quaternion _cameraTargetRotation;
        #endregion members

        #region properties
            public abstract string Name { get; }

            public int Priority
            {
                get { return this._priority; }
            }

            public float TransitionLerpT
            {
                get { return _transitionLerpT; }
            }
        #endregion properties

        #region constructors
            public virtual void Initialize() { }
        #endregion constructors

        #region methods
            protected abstract void CalculateModification(ICameraState cameraState, float deltaTime);

            public void ModifiyCamera(ICameraState cameraState, float deltaTime)
            {
                CalculateModification(cameraState, deltaTime);

                if (this._transitioning == true)
                {
                    Vector3 originalPosition = cameraState.Position;
                    Quaternion originalRotation = cameraState.Rotation;

                    cameraState.Position = Vector3.Lerp(originalPosition, this._cameraTargetPosition, this._transitionLerpT);

                    Vector3 directionFrom = originalRotation * Vector3.forward;
                    Vector3 directionTo = this._cameraTargetRotation * Vector3.forward;
                    Vector3 actualRotation = Vector3.Lerp(directionFrom, directionTo, this._transitionLerpT);
                    cameraState.Rotation = Quaternion.LookRotation(actualRotation.normalized, Vector3.up);
                }
                else
                {
                    cameraState.Position = this._cameraTargetPosition;
                    cameraState.Rotation = this._cameraTargetRotation;
                }
            }

            public virtual bool Enable()
            {
                this._transitionIntoEnabledDuration = Mathf.Max(0, this._transitionIntoEnabledDuration);

                if (CameraSystem.Instance == null)
                {
                    return false;
                }

                CameraSystem.Instance.AddModifier(this);

                if (Mathf.Approximately(this._transitionIntoEnabledDuration, 0.0f) == false)
                {
                    StartCoroutine("DoTransitionIn", this._transitionIntoEnabledDuration);
                }

                return true;
            }

            public virtual void Disable()
            {
                this._transitionIntoDisabledDuration = Mathf.Max(0, this._transitionIntoDisabledDuration);
                if (Mathf.Approximately(this._transitionIntoDisabledDuration, 0.0f) == false)
                {
                    StartCoroutine("DoTransitionOut", this._transitionIntoDisabledDuration);
                }
                else
                {
                    CameraSystem.Instance.RemoveModifier(this);
                }
            }

            public virtual void Cleanup() { }

            private IEnumerator DoTransitionIn(float transitionDuration)
            {
                this._transitioning = true;

                float elapsed = 0.0f;
                while (elapsed < transitionDuration)
                {
                    elapsed += Time.deltaTime;
                    float curveSamplePosition = Mathf.Clamp01(elapsed / transitionDuration);
                    this._transitionLerpT = CameraSystem.Instance.CameraInterpolationCurve.Evaluate(curveSamplePosition);

                    yield return null;
                }

                this._transitionLerpT = 1.0f;
                this._transitioning = false;
            }

            private IEnumerator DoTransitionOut(float transitionDuration)
            {
                this._transitioning = true;

                float elapsed = 0.0f;
                while (elapsed < transitionDuration)
                {
                    elapsed += Time.deltaTime;
                    float curveSamplePosition = 1.0f - Mathf.Clamp01(elapsed / transitionDuration);
                    this._transitionLerpT = CameraSystem.Instance.CameraInterpolationCurve.Evaluate(curveSamplePosition);
                    yield return null;
                }

                this._transitionLerpT = 0.0f;
                this._transitioning = false;

                CameraSystem.Instance.RemoveModifier(this);

                Cleanup();
            }
        #endregion methods
    }
}