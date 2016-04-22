using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace StrayTech
{
    /// <summary>
    /// Master control component of the camera system.
    /// </summary>
    [RenderHierarchyIcon("Assets/StrayTech/Camera System/Graphics/CameraSystem.png")]
    public class CameraSystem : MonoBehaviourSingleton<CameraSystem>
    {
        #region inner types
            public enum CameraStateEnum
            {
                Isometric = 0,
                Spline = 1,
                FirstPerson = 2,
                ThirdPerson = 3,
                Animated = 4,
                Pivot = 5,
                Parented = 6
            }

            public enum StateTransitionType
            {
                Interpolation = 0,
                Crossfade = 1,
                Instant = 2
            }

            public enum StateTransitionTypeInternal
            {
                Interpolation = 0,
                Crossfade = 1,
                Instant = 2,
                InterpolatedCrossfade = 3
            }

            public enum CameraSystemStatus
            {
                Active,
                Transitioning,
                Inactive
            }

            [Serializable]
            public class UserDefinedFlag
            {
                #region members
                    [SerializeField]
                    private string _name;
                    [SerializeField]
                    private bool _value = false;
                #endregion members

                #region properties
                    public string Name { get { return this._name; } set { this._name = value; } }
                    public bool Value { get { return this._value; } set { this._value = value; } }
                #endregion properties
            }
        #endregion inner types        
                                      
        #region inspector members
            [SerializeField]
            [Tooltip("The target for most of the camera states that require a target.")]
            private Transform _cameraTarget;

            [SerializeField]
            [Tooltip("The default Camera State.")]
            private CameraStateDefinition _defaultCameraState;

            [SerializeField]
            [Tooltip("Use FixedUpdate for physics based camera tracking.")]
            private bool _useFixedUpdate = false;

            [CustomAttributes.Uneditable]
            [Tooltip("The current camera state.")]
            public string _debugCurrentStateName = "None";

            [SerializeField]
            [Tooltip("User defined flags.")]
            private List<UserDefinedFlag> _userDefinedFlags = new List<UserDefinedFlag>();
        #endregion inspector members

        #region members
            private CameraSystemStatus _systemStatus = CameraSystemStatus.Inactive;
        
            private Camera _defaultCamera = null;
            private Camera _currentCamera = null;
            private Camera _nextCamera = null;

            private float _stateTransitionRamp = 1.0f;
            private StateTransitionTypeInternal _currentTransitionType;

            private LinkedList<CameraStateDefinition> _stateDefinitionHistory = new LinkedList<CameraStateDefinition>();
            private Queue<CameraStateDefinition> _stateDefinitionsToAdd = new Queue<CameraStateDefinition>();
            private Queue<CameraStateDefinition> _stateDefinitionsToRemove = new Queue<CameraStateDefinition>();

            private CameraStateDefinition _currentCameraStateDefinition = null;
            private CameraStateDefinition _nextCameraStateDefinition = null;
            private CameraStateDefinition _currentTransitionHost = null;
            private bool _transitionInteruptTransition = false;

            private List<CameraStateModifierBase> _cameraModifiers = new List<CameraStateModifierBase>();
            
            private RenderTexture _cameraRenderTexture;
            private CrossfadePostProcess _crossfadePostProcess = null;

            private bool _shouldUpdate = true;

            private Vector3 _cachedCameraPosition;
            private Quaternion _cachedCameraRotation;

            private Dictionary<string, bool> _userDefinedFlagsLookup = new Dictionary<string, bool>();

            private AnimationCurve _cameraInterpolationCurve;
        #endregion members

        #region properties
            public CameraSystemStatus SystemStatus { get { return this._systemStatus; } }
            public CameraStateDefinition CurrentCameraStateDefinition { get { return this._currentCameraStateDefinition; } }
            public CameraStateDefinition NextCameraStateDefinition { get { return this._nextCameraStateDefinition; } }
            public StateTransitionTypeInternal CurrentTransitionType { get { return _currentTransitionType; } }

            public Camera CurrentCamera { get { return this._currentCamera; } }
            public Camera NextCamera { get { return this._nextCamera; } }

            public AnimationCurve CameraInterpolationCurve { get { return this._cameraInterpolationCurve; } }
            public float CurrentInterpolationCurveSample { get { return this._cameraInterpolationCurve.Evaluate(Mathf.Clamp01(this._stateTransitionRamp)); } }

            public RenderTexture CameraRenderTexture { get { return this._cameraRenderTexture; } }

            public List<CameraStateModifierBase> CameraStateModifiers { get { return this._cameraModifiers; } }

            public bool ShouldUpdate
            {
                get { return this._shouldUpdate; }
                set { this._shouldUpdate = value; }
            }

            public Transform CameraTarget
            {
                get { return this._cameraTarget; }
                set { this._cameraTarget = value; }
            }
        #endregion properties

        #region constructors
            protected override void Awake()
            {
                base.Awake();
                
                this._defaultCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                this._currentCamera = this._defaultCamera;

                this._crossfadePostProcess = this._currentCamera.gameObject.AddOrGetComponent<CrossfadePostProcess>();
                this._crossfadePostProcess.enabled = false;

                this._cameraRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBHalf);

                this._cameraInterpolationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CacheUserDefinedFlags();

                if (this._defaultCameraState != null)
                {
                    RegisterCameraState(this._defaultCameraState);
                }
            }
        #endregion constructors

        #region methods
            /// <summary>
            /// Normal Update.
            /// </summary>
            private void Update()
            {
                if (this._useFixedUpdate == false)
                {
                    DoCameraUpdate(Time.deltaTime);
                }
            }

            /// <summary>
            /// Physics time update.
            /// </summary>
            private void FixedUpdate()
            {
                if (this._useFixedUpdate == true)
                {
                    DoCameraUpdate(Time.fixedDeltaTime);
                }
            }

            private void DoCameraUpdate(float deltaTime)
            {
                if (this._shouldUpdate == false)
                {
                    return;
                }

                if (this._currentCameraStateDefinition == null)
                {
                    return;
                }

                this._debugCurrentStateName = this._currentCameraStateDefinition.State.ToString();

                ManageQueuedTransitions();

                this._currentCameraStateDefinition.State.UpdateCamera(deltaTime);
                if (this._currentCameraStateDefinition.State.AllowsModifiers == true)
                {
                    for (int i = 0; i < this._cameraModifiers.Count; i++)
                    {
                        this._cameraModifiers[i].ModifiyCamera(this._currentCameraStateDefinition.State, deltaTime);
                    }
                }

                if (this._systemStatus == CameraSystemStatus.Transitioning)
                {
                    this._nextCameraStateDefinition.State.UpdateCamera(deltaTime);
                    if (this._nextCameraStateDefinition.State.AllowsModifiers == true)
                    {
                        for (int i = 0; i < this._cameraModifiers.Count; i++)
                        {
                            this._cameraModifiers[i].ModifiyCamera(this._nextCameraStateDefinition.State, deltaTime);
                        }
                    }
                }
                else
                {
                    this._currentCamera.transform.position = this._currentCameraStateDefinition.State.Position;
                    this._currentCamera.transform.rotation = this._currentCameraStateDefinition.State.Rotation;

                    if (CameraCollision.Instance != null && this._currentCameraStateDefinition.State.StateSettings.UseCameraCollision == true)
                    {
                        CameraCollision.Instance.PreventCameraCollision(this._currentCamera);
                    }
                }
            }

            private void LateUpdate()
            {
                if (this._systemStatus == CameraSystemStatus.Transitioning)
                {
                    switch (this._currentTransitionType)
                    {
                        case StateTransitionTypeInternal.Interpolation:
                            HandleInterpolationTransition();
                            break;
                        case StateTransitionTypeInternal.Crossfade:
                            HandleCrossfadeTransition();
                            break;
                        case StateTransitionTypeInternal.Instant:
                            HandleInstantTransition();
                            break;
                        case StateTransitionTypeInternal.InterpolatedCrossfade:
                            HandleInterpolatedCrossfadeTransition();
                            break;
                        default:
                            break;
                    }
                }
            }

            /// <summary>
            /// Register a new camera state.
            /// </summary>
            /// <param name="definition"></param>
            public void RegisterCameraState(CameraStateDefinition newState)
            {
                if (this._shouldUpdate == false)
                {
                    return;
                }

                if(this._currentCameraStateDefinition == null)
                {
                    this._stateDefinitionHistory.AddLast(newState);
                    this._currentTransitionHost = newState;
                    ActivateTransition(newState, newState);
                    return;
                }

                this._stateDefinitionsToAdd.Enqueue(newState);
            }

            /// <summary>
            /// Unregister a camera state.
            /// </summary>
            /// <param name="definition"></param>
            public void UnregisterCameraState(CameraStateDefinition oldState)
            {
                if (this._shouldUpdate == false)
                {
                    return;
                }

                if (this._stateDefinitionHistory.Contains(oldState) == false)
                {
                    return;
                }

                this._stateDefinitionsToRemove.Enqueue(oldState);
            }

            /// <summary>
            /// Manage transitions and switch system state.
            /// </summary>
            private void ManageQueuedTransitions()
            {
                if (this._systemStatus == CameraSystemStatus.Transitioning && (this._currentTransitionType == StateTransitionTypeInternal.Crossfade || this._currentTransitionType == StateTransitionTypeInternal.InterpolatedCrossfade))
                {
                    return;
                }

                // Process volumes that were entered.
                if (this._stateDefinitionsToAdd.Count > 0)
                {
                    CameraStateDefinition newState = this._stateDefinitionsToAdd.Dequeue();
                    this._stateDefinitionHistory.AddLast(newState);
                    if (this._stateDefinitionHistory.Count == 1 || (this._stateDefinitionHistory.Count > 1 && this._stateDefinitionHistory.Last.Previous.Value != newState))
                    {
                        ActivateTransition(newState, newState);
                    }
                }
                
                if (this._stateDefinitionsToRemove.Count > 0)
                {
                    // Process volumes that were exited.
                    if (this._stateDefinitionHistory.Last() == this._stateDefinitionsToRemove.Peek())
                    {
                        // If Transitioning - Touched a volume but then left it during the transition.
                        // If Not Transitioning - Leaving one volume and reverting to last entered volume if it exists.
                        CameraStateDefinition fromState = this._stateDefinitionsToRemove.Peek();
                        this._stateDefinitionHistory.RemoveLast();

                        if (this._stateDefinitionHistory.Count > 0)
                        {
                            CameraStateDefinition toState = this._stateDefinitionHistory.Last();

                            if (this._stateDefinitionHistory.Count == 1)
                            {
                                ActivateTransition(toState, toState);
                            }
                            else
                            {
                                if (toState != fromState)
                                {
                                    ActivateTransition(toState, fromState);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Left a volume that was not currently influencing the camera.
                        this._stateDefinitionHistory.Remove(this._stateDefinitionsToRemove.Peek());
                    }

                    this._stateDefinitionsToRemove.Dequeue();
                }
            }

            private void ActivateTransition(CameraStateDefinition toState, CameraStateDefinition transitionHost)
            {
                if (this._systemStatus == CameraSystemStatus.Transitioning)
                {
                    this._transitionInteruptTransition = true;
                    //FinalizeTransition();
                }
                else
                {
                    this._transitionInteruptTransition = false;
                }

                this._systemStatus = CameraSystemStatus.Transitioning;

                this._nextCameraStateDefinition = toState;
                this._nextCamera = (this._nextCameraStateDefinition.Camera != null) ? this._nextCameraStateDefinition.Camera : this._defaultCamera;
                this._currentTransitionHost = transitionHost;

                if (this._currentTransitionHost == this._nextCameraStateDefinition)
                {
                    this._currentTransitionHost.InitializeState();
                }

                if (this._currentCamera != null && this._nextCamera != null && this._currentCamera != this._nextCamera)
                {
                    AudioListener currentAudioListener = this._currentCamera.gameObject.GetComponent<AudioListener>();
                    if (currentAudioListener != null) currentAudioListener.enabled = false;
                    AudioListener nextAudioListener = this._nextCamera.gameObject.GetComponent<AudioListener>();
                    if (nextAudioListener != null) nextAudioListener.enabled = true;
                }

                if (this._currentTransitionHost.TransitionType == StateTransitionType.Instant || Mathf.Approximately(this._currentTransitionHost.TransitionDuration, 0) == true || this._currentCameraStateDefinition == null)
                {
                    this._currentTransitionType = StateTransitionTypeInternal.Instant;
                }
                else
                {
                    if (this._currentTransitionHost.TransitionType == StateTransitionType.Crossfade)
                    {
                        this._currentTransitionType = StateTransitionTypeInternal.Crossfade;
                    }
                    else
                    {
                        if (this._currentCamera == this._nextCamera)
                        {
                            this._currentTransitionType = StateTransitionTypeInternal.Interpolation;
                        }
                        else
                        {
                            this._currentTransitionType = StateTransitionTypeInternal.InterpolatedCrossfade;
                        }
                    }

                    this._cachedCameraPosition = this._currentCamera.transform.position;
                    this._cachedCameraRotation = this._currentCamera.transform.rotation;

                    this._stateTransitionRamp = 0.0f;
                }
            }

            /// <summary>
            /// Instant Transition handling.
            /// </summary>
            private void HandleInstantTransition()
            {
                FinalizeTransition();
            }

            /// <summary>
            /// Interpolated transition handling.
            /// </summary>
            private void HandleInterpolationTransition()
            {
                this._stateTransitionRamp += Time.unscaledDeltaTime / this._currentTransitionHost.TransitionDuration;
                this._stateTransitionRamp = Mathf.Clamp01(this._stateTransitionRamp);

                if (Mathf.Approximately(this._stateTransitionRamp, 1.0f) == true)
                {
                    FinalizeTransition();
                    return;
                }

                float sampledTValue = CurrentInterpolationCurveSample;

                if (this._transitionInteruptTransition == true)
                {
                    this._currentCamera.transform.position = Vector3.Lerp(this._cachedCameraPosition, this._nextCameraStateDefinition.State.Position, sampledTValue);
                    this._currentCamera.transform.rotation = Quaternion.Slerp(this._cachedCameraRotation, this._nextCameraStateDefinition.State.Rotation, sampledTValue);
                }
                else
                {
                    this._currentCamera.transform.position = Vector3.Lerp(this._currentCameraStateDefinition.State.Position, this._nextCameraStateDefinition.State.Position, sampledTValue);
                    this._currentCamera.transform.rotation = Quaternion.Slerp(this._currentCameraStateDefinition.State.Rotation, this._nextCameraStateDefinition.State.Rotation, sampledTValue);
                }

                if (CameraCollision.Instance != null && this._nextCameraStateDefinition != null && this._nextCameraStateDefinition.State.StateSettings.UseCameraCollision == true)
                {
                    CameraCollision.Instance.PreventCameraCollision(this._nextCamera);
                }
            }

            /// <summary>
            /// Crossfade transition handling.
            /// </summary>
            private void HandleCrossfadeTransition()
            {
                this._stateTransitionRamp += Time.unscaledDeltaTime / this._currentTransitionHost.TransitionDuration;
                this._stateTransitionRamp = Mathf.Clamp01(this._stateTransitionRamp);

                this._nextCamera.transform.position = this._nextCameraStateDefinition.State.Position;
                this._nextCamera.transform.rotation = this._nextCameraStateDefinition.State.Rotation;

                if (CameraCollision.Instance != null && this._nextCameraStateDefinition != null && this._nextCameraStateDefinition.State.StateSettings.UseCameraCollision == true)
                {
                    CameraCollision.Instance.PreventCameraCollision(this._nextCamera);
                }

                if (Mathf.Approximately(this._stateTransitionRamp, 1.0f) == true)
                {
                    FinalizeTransition();
                    return;
                }

                this._crossfadePostProcess.enabled = false;
                this._nextCamera.targetTexture = this._cameraRenderTexture;
                this._nextCamera.Render();
                this._nextCamera.targetTexture = null;
                this._crossfadePostProcess.enabled = true;

                if (this._transitionInteruptTransition == true)
                {
                    this._currentCamera.transform.position = this._cachedCameraPosition;
                    this._currentCamera.transform.rotation = this._cachedCameraRotation;
                }
                else
                {
                    this._currentCamera.transform.position = this._currentCameraStateDefinition.State.Position;
                    this._currentCamera.transform.rotation = this._currentCameraStateDefinition.State.Rotation;
                }

                if (CameraCollision.Instance != null && this._currentCameraStateDefinition != null && this._currentCameraStateDefinition.State.StateSettings.UseCameraCollision == true)
                {
                    CameraCollision.Instance.PreventCameraCollision(this._currentCamera);
                }
            }

            /// <summary>
            /// Interpolated Crossfade Transition handling
            /// </summary>
            private void HandleInterpolatedCrossfadeTransition()
            {
                this._stateTransitionRamp += Time.unscaledDeltaTime / this._currentTransitionHost.TransitionDuration;
                this._stateTransitionRamp = Mathf.Clamp01(this._stateTransitionRamp);

                if (Mathf.Approximately(this._stateTransitionRamp, 1.0f) == true)
                {
                    FinalizeTransition();
                    return;
                }

                float sampledTValue = CurrentInterpolationCurveSample;

                Vector3 targetPosition;
                Quaternion targetRotation;

                if (this._transitionInteruptTransition == true)
                {
                    targetPosition = Vector3.Lerp(this._cachedCameraPosition, this._nextCameraStateDefinition.State.Position, sampledTValue);
                    targetRotation = Quaternion.Slerp(this._cachedCameraRotation, this._nextCameraStateDefinition.State.Rotation, sampledTValue);
                }
                else
                {
                    targetPosition = Vector3.Lerp(this._currentCameraStateDefinition.State.Position, this._nextCameraStateDefinition.State.Position, sampledTValue);
                    targetRotation = Quaternion.Slerp(this._currentCameraStateDefinition.State.Rotation, this._nextCameraStateDefinition.State.Rotation, sampledTValue);
                }

                this._currentCamera.transform.position = targetPosition;
                this._currentCamera.transform.rotation = targetRotation;

                if (CameraCollision.Instance != null && this._currentCameraStateDefinition != null && this._currentCameraStateDefinition.State.StateSettings.UseCameraCollision == true)
                {
                    CameraCollision.Instance.PreventCameraCollision(this._currentCamera);
                }

                this._nextCamera.transform.position = this._currentCamera.transform.position;
                this._nextCamera.transform.rotation = this._currentCamera.transform.rotation;

                this._crossfadePostProcess.enabled = false;
                this._nextCamera.targetTexture = this._cameraRenderTexture;
                this._nextCamera.Render();
                this._nextCamera.targetTexture = null;
                this._crossfadePostProcess.enabled = true;
            }

            /// <summary>
            /// Finalize the transition and ensure clean state change.
            /// </summary>
            private void FinalizeTransition()
            {
                if (this._systemStatus == CameraSystemStatus.Transitioning)
                {
                    if (this._currentCameraStateDefinition != null)
                    {
                        this._currentCameraStateDefinition.State.Cleanup();
                    }

                    this._currentCameraStateDefinition = this._nextCameraStateDefinition;
                    this._nextCameraStateDefinition = null;

                    if (this._nextCamera != this._currentCamera)
                    {
                        if (this._currentCamera != null)
                        {
                            this._currentCamera.enabled = false;
                        }

                        this._currentCamera = this._nextCamera;
                        this._currentCamera.enabled = true;

                        this._crossfadePostProcess.enabled = false;
                        this._crossfadePostProcess = this._currentCamera.gameObject.AddOrGetComponent<CrossfadePostProcess>();

                        this._nextCamera = null;
                    }

                    if (this._transitionInteruptTransition == false)
                    {
                        this._currentCamera.transform.position = this._currentCameraStateDefinition.State.Position;
                        this._currentCamera.transform.rotation = this._currentCameraStateDefinition.State.Rotation;
                    }

                    this._stateTransitionRamp = 0.0f;
                    this._crossfadePostProcess.enabled = false;
                    this._currentTransitionHost = null;
                    this._transitionInteruptTransition = false;

                    this._systemStatus = CameraSystemStatus.Active;
                }
            }

            /// <summary>
            /// Adds a camera modifier that can be used with any Camera State that supports modifiers.
            /// </summary>
            /// <param name="modifier"></param>
            public void AddModifier(CameraStateModifierBase modifier)
            {
                if (this._cameraModifiers.Contains(modifier) == true)
                {
                    Debug.LogFormat("Modifier already in use: {0}", modifier);
                    return;
                }

                this._cameraModifiers.Add(modifier);
                modifier.Initialize();

                this._cameraModifiers = this._cameraModifiers.OrderBy(m => m.Priority).ToList();
            }

            /// <summary>
            /// Removes a camera modifier if it is in use.
            /// </summary>
            /// <param name="modifier"></param>
            public void RemoveModifier(CameraStateModifierBase modifier)
            {
                if (this._cameraModifiers.Contains(modifier) == false)
                {
                    //Debug.LogFormat("Modifier not in use: {0}", modifier);
                    return;
                }

                this._cameraModifiers.Remove(modifier);
            }

            /// <summary>
            /// Change the current camera's field of view.
            /// </summary>
            /// <param name="fov"></param>
            public void ChangeCameraFOV(float fov)
            {
                this._currentCamera.fieldOfView = fov;
            }

            /// <summary>
            /// Cache user defined tags into a dictionary for fast retrieval.
            /// </summary>
            private void CacheUserDefinedFlags()
            {
                foreach(UserDefinedFlag flag in this._userDefinedFlags)
                {
                    this._userDefinedFlagsLookup.Add(flag.Name, flag.Value);
                }
            }

            /// <summary>
            /// Gets the value of a user defined flag if it has been set.
            /// </summary>
            /// <param name="flagName"></param>
            /// <returns></returns>
            public bool GetUserDefinedFlagValue(string flagName)
            {
                bool value = false;
                if (this._userDefinedFlagsLookup.TryGetValue(flagName, out value))
                {
                    return value;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Sets the value of an existing user defined flag or creates a new one.
            /// </summary>
            /// <param name="flagName"></param>
            /// <param name="flagValue"></param>
            public void SetUserDefinedFlagValue(string flagName, bool flagValue)
            {
                bool value = false;
                if (this._userDefinedFlagsLookup.TryGetValue(flagName, out value))
                {
                    this._userDefinedFlagsLookup[flagName] = flagValue;
                }
                else
                {
                    this._userDefinedFlags.Add(new UserDefinedFlag() { Name = flagName, Value = flagValue });
                    this._userDefinedFlagsLookup.Add(flagName, flagValue);
                }
            }
        #endregion methods
    }
}