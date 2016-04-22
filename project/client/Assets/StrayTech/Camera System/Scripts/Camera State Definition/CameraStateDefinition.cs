using UnityEngine;
using System.Collections;
using System;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    /// <summary>
    /// A data class which allows you to define the parameters needed by a Camera State to function. 
    /// </summary>
    [Serializable]
    [RenderHierarchyIcon("Assets/StrayTech/Camera System/Graphics/CameraRig.png")]
    public class CameraStateDefinition : MonoBehaviour
    {
        #region inspector members
            [SerializeField]
            [Tooltip("The camera state to use.")]
            private CameraSystem.CameraStateEnum _cameraState = CameraSystem.CameraStateEnum.Isometric;

            [SerializeField]
            [Tooltip("The type of transition to the new camera state.")]
            private CameraSystem.StateTransitionType _transitionType = CameraSystem.StateTransitionType.Interpolation;

            [SerializeField]
            [Tooltip("The duration of the transition to the new camera state.")]
            private float _transitionDuration = 1.0f;

            [SerializeField]
            [Tooltip("The Camera GameObject to use for this state.")]
            private Camera _camera = null;

            [SerializeField]
            private FirstPersonCameraStateSettings _firstPersonStateSettings;

            [SerializeField]
            private IsometricCameraStateSettings _isometricStateSettings;

            [SerializeField]
            private SplineCameraStateSettings _splineStateSettings;

            [SerializeField]
            private ThirdPersonCameraStateSettings _thirdPersonStateSettings;

            [SerializeField]
            private AnimatedCameraStateSettings _animatedCameraStateSettings;

            [SerializeField]
            private PivotCameraStateSettings _pivotCameraStateSettings;

            [SerializeField]
            private ParentedCameraStateSettings _parentedCameraStateSettings;
        #endregion inspector members

        #region members
            private ICameraState _state;
        #endregion members

        #region properties
            public ICameraState State { get { return _state; } }
            public CameraSystem.StateTransitionType TransitionType { get { return _transitionType; } }
            public float TransitionDuration { get { return _transitionDuration; } }
            public Camera Camera { get { return this._camera; } }
        #endregion properties

        #region constructors
            private void Start()
            {
                if (CameraSystem.Instance == null)
                {
                    Debug.LogError("There was no CameraMasterControl found in the scene!");
                    return;
                }
            }
        #endregion constructors

        #region methods
            public void InitializeState()
            {
                if (this._state != null)
                {
                    this._state.Cleanup();
                    this._state = null;
                }

                switch (this._cameraState)
                {
                    case CameraSystem.CameraStateEnum.Isometric:
                        this._state = new IsometricCamera(this._isometricStateSettings);
                        break;
                    case CameraSystem.CameraStateEnum.Spline:
                        this._state = new SplineCamera(this._splineStateSettings);
                        break;
                    case CameraSystem.CameraStateEnum.FirstPerson:
                        this._state = new FirstPersonCamera(this._firstPersonStateSettings);
                        break;
                    case CameraSystem.CameraStateEnum.ThirdPerson:
                        this._state = new ThirdPersonCamera(this._thirdPersonStateSettings);
                        break;
                    case CameraSystem.CameraStateEnum.Animated:
                        this._state = new AnimatedCamera(this._animatedCameraStateSettings);
                        break;
                    case CameraSystem.CameraStateEnum.Pivot:
                        this._state = new PivotCamera(this._pivotCameraStateSettings);
                        break;
                    case CameraSystem.CameraStateEnum.Parented:
                        this._state = new ParentedCamera(this._parentedCameraStateSettings);
                        break;
                    default:
                        Debug.LogErrorFormat("No args present for CameraStateEnum {0}", this._cameraState);
                        break;
                }
            }

            public void AddCameraStateTriggerChild()
            {
                GameObject childTrigger = new GameObject("Camera State Trigger", typeof(BoxCollider), typeof(EditorVisibleVolume), typeof(EnforceUnitScale), typeof(CameraStateTransitionTrigger));
                childTrigger.transform.parent = this.transform;

                CameraStateTransitionTrigger trigger = childTrigger.GetComponent<CameraStateTransitionTrigger>();
                trigger.TargetCameraStateDefinition = this;

                BoxCollider collider = childTrigger.GetComponent<BoxCollider>();
                collider.isTrigger = true;

                EditorVisibleVolume volume = childTrigger.GetComponent<EditorVisibleVolume>();
                volume.VolumeColor = new Color(0, 1, 0, 0.25f);
            }

            public void AddCameraSplineChild()
            {
                GameObject gameObject = new GameObject("Camera Spline", typeof(BezierSpline));
                gameObject.transform.parent = this.transform;

                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }
        #endregion methods
    }
}