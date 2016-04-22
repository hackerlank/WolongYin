using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// A custom property drawer for ParentedCameraStateSettings. 
    /// </summary>
    [CustomPropertyDrawer(typeof(ParentedCameraStateSettings))]
    public class ParentedCameraStateSettingsPropertyDrawer : PropertyDrawer
    {
        #region members
            private SerializedProperty _parentField;
            private SerializedProperty _positionOffsetField;
            private SerializedProperty _rotationOffsetField;
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
                    this._parentField = property.FindPropertyRelativeExplosive("_parent");
                    this._positionOffsetField = property.FindPropertyRelativeExplosive("_positionOffset");
                    this._rotationOffsetField = property.FindPropertyRelativeExplosive("_rotationOffset");
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

                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._parentField)), this._parentField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._positionOffsetField)), this._positionOffsetField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._rotationOffsetField)), this._rotationOffsetField);
                EditorGUI.PropertyField(EditorExtensions.ExtractSpace(ref canvas, EditorGUI.GetPropertyHeight(this._useCameraCollisionField)), this._useCameraCollisionField);
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

                runningHeight += EditorGUI.GetPropertyHeight(this._parentField);
                runningHeight += EditorGUI.GetPropertyHeight(this._positionOffsetField);
                runningHeight += EditorGUI.GetPropertyHeight(this._rotationOffsetField);
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