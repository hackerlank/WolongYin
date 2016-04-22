using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// A custom property drawer for FirstPersonCameraStateSettings. 
    /// </summary>
    [CustomPropertyDrawer(typeof(FirstPersonCameraStateSettings))]
    public class FirstPersonCameraStateSettingsPropertyDrawer : PropertyDrawer
    {
        #region members
            private SerializedProperty _positionRootTransformField;
            private SerializedProperty _characterTransformField;
            private SerializedProperty _positionOffsetField;
            private SerializedProperty _pitchRangeField;
            private SerializedProperty _mouseLookSensitivityField;
            private SerializedProperty _mouseSmoothingField;
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

                //Perform change check so we can apply modified properties if the user makes a change. 
                EditorGUI.BeginChangeCheck();
                {
                    DrawCustomGUI(canvas, property, label);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            private bool CacheFields(SerializedProperty property, GUIContent label)
            {
                // if you refactor a class, it's field names might change but that won't be reflected here.
                // fail gracefully by returning false instead of emitting hundreds of hard exceptions.
                try
                {
                    this._positionRootTransformField = property.FindPropertyRelativeExplosive("_positionRootTransform");
                    this._characterTransformField = property.FindPropertyRelativeExplosive("_characterTransform");
                    this._positionOffsetField = property.FindPropertyRelativeExplosive("_positionOffset");
                    this._pitchRangeField = property.FindPropertyRelativeExplosive("_pitchRange");
                    this._mouseLookSensitivityField = property.FindPropertyRelativeExplosive("_mouseLookSensitivity");
                    this._mouseSmoothingField = property.FindPropertyRelativeExplosive("_mouseSmoothing");
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

                //Render all of the property fields. 
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._positionRootTransformField)), this._positionRootTransformField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._characterTransformField)), this._characterTransformField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._positionOffsetField)), this._positionOffsetField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._pitchRangeField)), this._pitchRangeField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._mouseLookSensitivityField)), this._mouseLookSensitivityField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._mouseSmoothingField)), this._mouseSmoothingField);
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

                runningHeight += EditorGUI.GetPropertyHeight(this._positionRootTransformField);
                runningHeight += EditorGUI.GetPropertyHeight(this._characterTransformField);
                runningHeight += EditorGUI.GetPropertyHeight(this._positionOffsetField);
                runningHeight += EditorGUI.GetPropertyHeight(this._pitchRangeField);
                runningHeight += EditorGUI.GetPropertyHeight(this._mouseLookSensitivityField);
                runningHeight += EditorGUI.GetPropertyHeight(this._mouseSmoothingField);

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