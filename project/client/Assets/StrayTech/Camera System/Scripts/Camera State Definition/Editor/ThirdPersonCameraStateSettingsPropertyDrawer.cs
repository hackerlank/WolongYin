using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// A custom property drawer for ThirdPersonCameraStateSettings. 
    /// </summary>
    [CustomPropertyDrawer(typeof(ThirdPersonCameraStateSettings))]
    public class ThirdPersonCameraStateSettingsPropertyDrawer : PropertyDrawer
    {
        #region members
            private SerializedProperty _mouseOrbitField;
            private SerializedProperty _targetOffsetField;
            private SerializedProperty _mouseOrbitDistanceField;
            private SerializedProperty _mousePitchRangeField;
            private SerializedProperty _mouseSensitivityField;
            private SerializedProperty _mouseInvertYField;
            private SerializedProperty _motionSmoothingField;
            private SerializedProperty _useCameraCollisionField;
        #endregion members

        #region methods
            public override void OnGUI(Rect canvas, SerializedProperty property, GUIContent label)
            {
                // fail gracefully if CacheFields throws an exception.
                if (this.CacheFields(property, label) == false)
                {
                    this.DrawCacheFieldsFailureMessage(canvas, label);
                    return;
                }

                DrawCustomGUI(canvas, property, label);
            }

            private bool CacheFields(SerializedProperty property, GUIContent label)
            {
                // if you refactor a class, it's field names might change but that won't be reflected here.
                // fail gracefully by returning false instead of emitting hundreds of hard exceptions.
                try
                {
                    this._mouseOrbitField = property.FindPropertyRelativeExplosive("_mouseOrbit");
                    this._targetOffsetField = property.FindPropertyRelativeExplosive("_targetOffset");
                    this._mouseOrbitDistanceField = property.FindPropertyRelativeExplosive("_mouseOrbitDistance");
                    this._mousePitchRangeField = property.FindPropertyRelativeExplosive("_mousePitchRange");
                    this._mouseSensitivityField = property.FindPropertyRelativeExplosive("_mouseSensitivity");
                    this._mouseInvertYField = property.FindPropertyRelativeExplosive("_mouseInvertY");
                    this._motionSmoothingField = property.FindPropertyRelativeExplosive("_motionSmoothing");
                    this._useCameraCollisionField = property.FindPropertyRelativeExplosive("_useCameraCollision");
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
            private void DrawCustomGUI(Rect canvas, SerializedProperty property, GUIContent label)
            {
                EditorExtensions.RenderBackgroundRect(canvas, GetPropertyHeight(property, label), property.name);

                //Extract space to make up for the title! 
                EditorExtensions.ExtractSpace(ref canvas, 19f);

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._mouseOrbitField)), this._mouseOrbitField);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.BeginChangeCheck();
                {
                    if (this._mouseOrbitField.boolValue == true)
                    {
                        EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._mouseOrbitDistanceField)), this._mouseOrbitDistanceField);
                        EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._mousePitchRangeField)), this._mousePitchRangeField);
                        EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._mouseSensitivityField)), this._mouseSensitivityField);
                        EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._mouseInvertYField)), this._mouseInvertYField);
                    }
                    else
                    {
                        EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._targetOffsetField)), this._targetOffsetField);
                    }
                    
                    EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._motionSmoothingField)), this._motionSmoothingField);
                    EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._useCameraCollisionField)), this._useCameraCollisionField);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            /// <summary>
            /// Return the property height 
            /// </summary>
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (this.CacheFields(property, label) == false)
                {
                    return EditorGUIUtility.singleLineHeight;
                }

                var runningHeight = 25f;

                runningHeight += EditorGUI.GetPropertyHeight(this._mouseOrbitField);
                if (this._mouseOrbitField.boolValue == true)
                {
                    runningHeight += EditorGUI.GetPropertyHeight(this._mouseOrbitDistanceField);
                    runningHeight += EditorGUI.GetPropertyHeight(this._mousePitchRangeField);
                    runningHeight += EditorGUI.GetPropertyHeight(this._mouseSensitivityField);
                    runningHeight += EditorGUI.GetPropertyHeight(this._mouseInvertYField);
                }
                else
                {
                    runningHeight += EditorGUI.GetPropertyHeight(this._targetOffsetField);
                }

                runningHeight += EditorGUI.GetPropertyHeight(this._motionSmoothingField);
                runningHeight += EditorGUI.GetPropertyHeight(this._useCameraCollisionField);

                return runningHeight;
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
            private void DrawCacheFieldsFailureMessage(Rect canvas, GUIContent label)
            {
                GUIColorHelper.DoWithColor(Color.red,
                    () =>
                    {
                        EditorGUI.LabelField(canvas, label.text, string.Format("{0}.CacheFields threw an exception: {1}", this.GetType().Name, this._cacheFieldsFailureMessage ?? string.Empty));
                    }
                );
            }
        #endregion template members
    }
}