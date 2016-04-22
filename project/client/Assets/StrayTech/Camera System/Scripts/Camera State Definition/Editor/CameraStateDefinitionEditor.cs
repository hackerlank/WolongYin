using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEditor;

namespace StrayTech
{
    /// <summary>
    /// Custom editor for CameraStateDefinition
    /// </summary>
    [CustomEditor(typeof(CameraStateDefinition))]
    public class CameraStateDefinitionEditor : Editor
    {
        #region members
            private SerializedProperty _cameraStateEnumField;
            private SerializedProperty _transitionTypeField;
            private SerializedProperty _transitionDurationField;
            private SerializedProperty _cameraField;

            private SerializedProperty _firstPersonStateSettingsField;
            private SerializedProperty _isometricStateSettingsField;
            private SerializedProperty _splineStateSettingsField;
            private SerializedProperty _thirdPersonStateSettingsField;
            private SerializedProperty _animatedCameraStateSettingsField;
            private SerializedProperty _pivotCameraStateSettingsField;
            private SerializedProperty _parentedCameraStateSettingsField;

            private CameraStateDefinition _inspected;
            private CameraSystem.CameraStateEnum _cameraState;
        #endregion members

        #region methods
            public override void OnInspectorGUI()
            {
                this._inspected = this.target as CameraStateDefinition;

                // fail gracefully if CacheFields throws an exception.
                if (this.CacheFields() == false)
                {
                    this.DrawCacheFieldsFailureMessage();
                    return;
                }

                EditorGUI.BeginChangeCheck();
                {
                    DrawCustomGUI();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                }

                DrawButtons();
            }

            /// <summary>
            /// Cache the serialized fields we need. 
            /// </summary>
            /// <param name="property"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            private bool CacheFields()
            {
                // if you refactor a class, it's field names might change but that won't be reflected here.
                // fail gracefully by returning false instead of emitting hundreds of hard exceptions.
                try
                {
                    this._cameraStateEnumField = serializedObject.FindPropertyExplosive("_cameraState");
                    this._transitionTypeField = serializedObject.FindPropertyExplosive("_transitionType");
                    this._transitionDurationField = serializedObject.FindPropertyExplosive("_transitionDuration");
                    this._cameraField = serializedObject.FindPropertyExplosive("_camera");

                    this._firstPersonStateSettingsField = serializedObject.FindPropertyExplosive("_firstPersonStateSettings");
                    this._isometricStateSettingsField = serializedObject.FindPropertyExplosive("_isometricStateSettings");
                    this._splineStateSettingsField = serializedObject.FindPropertyExplosive("_splineStateSettings");
                    this._thirdPersonStateSettingsField = serializedObject.FindPropertyExplosive("_thirdPersonStateSettings");
                    this._animatedCameraStateSettingsField = serializedObject.FindPropertyExplosive("_animatedCameraStateSettings");
                    this._pivotCameraStateSettingsField = serializedObject.FindPropertyExplosive("_pivotCameraStateSettings");
                    this._parentedCameraStateSettingsField = serializedObject.FindPropertyExplosive("_parentedCameraStateSettings");
                }
                catch (Exception e)
                {
                    this._cacheFieldsFailureMessage = e.Message;
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Render our custom GUI. 
            /// </summary>
            private void DrawCustomGUI()
            {
                EditorGUILayout.PropertyField(this._cameraStateEnumField);
                EditorGUILayout.PropertyField(this._transitionTypeField);

                if ((CameraSystem.StateTransitionType)this._transitionTypeField.enumValueIndex != CameraSystem.StateTransitionType.Instant)
                {
                    EditorGUILayout.PropertyField(this._transitionDurationField);
                }

                EditorGUILayout.PropertyField(this._cameraField);

                if (this._cameraField.objectReferenceValue != null && PrefabUtility.GetPrefabType((this._cameraField.objectReferenceValue as Camera).gameObject) == PrefabType.Prefab)
                {
                    this._cameraField.objectReferenceValue = null;
                }

                EditorGUILayout.Space();
                EditorTools.DrawDivider(6.0f);
                EditorGUILayout.Space();

                this._cameraState = (CameraSystem.CameraStateEnum)this._cameraStateEnumField.enumValueIndex;

                switch (this._cameraState)
                {
                    case CameraSystem.CameraStateEnum.Isometric:
                        {
                            EditorGUILayout.PropertyField(this._isometricStateSettingsField);
                        }
                        break;
                    case CameraSystem.CameraStateEnum.Spline:
                        {
                            EditorGUILayout.PropertyField(this._splineStateSettingsField);
                        }
                        break;
                    case CameraSystem.CameraStateEnum.FirstPerson:
                        {
                            EditorGUILayout.PropertyField(this._firstPersonStateSettingsField);
                        }
                        break;
                    case CameraSystem.CameraStateEnum.ThirdPerson:
                        {
                            EditorGUILayout.PropertyField(this._thirdPersonStateSettingsField);
                        }
                        break;
                    case CameraSystem.CameraStateEnum.Animated:
                        {
                            EditorGUILayout.PropertyField(this._animatedCameraStateSettingsField);
                        }
                        break;
                    case CameraSystem.CameraStateEnum.Pivot:
                        {
                            EditorGUILayout.PropertyField(this._pivotCameraStateSettingsField);
                        }
                        break;
                    case CameraSystem.CameraStateEnum.Parented:
                        {
                            EditorGUILayout.PropertyField(this._parentedCameraStateSettingsField);
                        }
                        break;
                    default:
                        {
                            EditorGUILayout.LabelField(string.Format("ERROR: Cannot render CameraStateEnum {0}", this._cameraState));
                        }
                        break;
                }
            }

            private void DrawButtons()
            {
                EditorTools.DrawDivider(12.0f);
                if (PrefabUtility.GetPrefabType(this._inspected.gameObject) != PrefabType.Prefab)
                {
                    if (Application.isPlaying == false)
                    {
                        if (GUILayout.Button("Add Camera State Trigger") == true)
                        {
                            this._inspected.AddCameraStateTriggerChild();
                        }

                        if (this._cameraState == CameraSystem.CameraStateEnum.Spline)
                        {
                            if (GUILayout.Button("Add Camera Spline") == true)
                            {
                                this._inspected.AddCameraSplineChild();
                            }
                        }
                    }
                }
            }
        #endregion methods

        #region template members
            /// <summary>
            /// An error message shown in the inspector when CacheFields throws an exception.
            /// </summary>
            private string _cacheFieldsFailureMessage = string.Empty;

            /// <summary>
            /// Display an error message caused by CacheFields throwing an exception.
            /// </summary>
            private void DrawCacheFieldsFailureMessage()
            {
                GUIColorHelper.DoWithColor(Color.red,
                    () =>
                    {
                        EditorGUILayout.LabelField(string.Format("{0}.CacheFields threw an exception: {1}", this.GetType().Name, this._cacheFieldsFailureMessage ?? string.Empty));
                    }
                );
            }
        #endregion template members
    }
}