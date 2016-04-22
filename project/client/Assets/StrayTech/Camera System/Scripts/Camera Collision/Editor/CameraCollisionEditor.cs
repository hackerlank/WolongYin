using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEditor;

namespace StrayTech
{
    /// <summary>
    /// Custom editor for CameraCollision
    /// </summary>
    [CustomEditor(typeof(CameraCollision))]
    public class CameraCollisionEditor : Editor
    {
        #region members
            private SerializedProperty _useCameraCollisionField;
            private SerializedProperty _testTypeField;
            private SerializedProperty _sphereRadiusField;
            private SerializedProperty _collisionLayerMaskField;
        #endregion members

        #region methods
            public override void OnInspectorGUI()
            {
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
                    this._useCameraCollisionField = serializedObject.FindPropertyExplosive("_useCameraCollision");
                    this._testTypeField = serializedObject.FindPropertyExplosive("_testType");
                    this._sphereRadiusField = serializedObject.FindPropertyExplosive("_sphereRadius");
                    this._collisionLayerMaskField = serializedObject.FindPropertyExplosive("_collisionLayerMask");
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
                EditorGUILayout.PropertyField(this._useCameraCollisionField);
                if (this._useCameraCollisionField.boolValue == true)
                {
                    EditorGUILayout.PropertyField(this._testTypeField);
                    if ((CameraCollision.CollisionTestType)this._testTypeField.enumValueIndex == CameraCollision.CollisionTestType.SphereCast)
                    {
                        EditorGUILayout.PropertyField(this._sphereRadiusField);
                    }
                    EditorGUILayout.PropertyField(this._collisionLayerMaskField);
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