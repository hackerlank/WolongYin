using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public class CameraShakeModifier : CameraStateModifierBase
    {
        #region inspector members
            [SerializeField]
            [Tooltip("Defines the duration and intensity multiplier of the camera shake.")]
            private AnimationCurve _shakeIntensityMultiplierCurve;

            [SerializeField]
            [Tooltip("The base intensity of the camera shake.")]
            private float _shakeIntensity = 3.0f;
        #endregion inspector members

        #region members
            private Vector3 _positionOffset = Vector3.zero;
            private Quaternion _rotationOffset = Quaternion.identity;

            private float _shakeDuration = 0.0f;
            private float _shakeLerpT = 0.0f;
        #endregion members

        #region properties
            public override string Name
            {
                get { return "Camera Shake Modifier"; }
            }
        #endregion properties

        #region methods
            protected override void CalculateModification(ICameraState cameraState, float deltaTime)
            {
                this._positionOffset += Random.insideUnitSphere * this._shakeIntensity * deltaTime;
                this._rotationOffset *= Quaternion.AngleAxis(this._shakeIntensity * 10.0f * deltaTime, Random.insideUnitSphere);

                this._cameraTargetPosition = Vector3.Lerp(cameraState.Position, cameraState.Position + this._positionOffset, this._shakeLerpT);
                this._cameraTargetRotation = Quaternion.Slerp(cameraState.Rotation, cameraState.Rotation * this._rotationOffset, this._shakeLerpT);
            }

            public override bool Enable()
            {
                this._shakeDuration = this._shakeIntensityMultiplierCurve.keys[this._shakeIntensityMultiplierCurve.length - 1].time;
                this.StartCoroutine(DoCurveBasedCameraShake());

                return base.Enable();
            }

            private IEnumerator DoCurveBasedCameraShake()
            {
                float elapsedTime = 0.0f;

                while (elapsedTime <= this._shakeDuration)
                {
                    elapsedTime += Time.deltaTime;

                    this._shakeLerpT = this._shakeIntensityMultiplierCurve.Evaluate(elapsedTime);

                    yield return null;
                }

                this.Disable();
            }
        #endregion methods
    }
}