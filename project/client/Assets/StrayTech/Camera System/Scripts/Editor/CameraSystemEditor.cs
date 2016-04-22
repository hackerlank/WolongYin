using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEditorInternal;

namespace StrayTech
{
    /// <summary>
    /// Custom editor for CameraStateDefinition
    /// </summary>
    [CustomEditor(typeof(CameraSystem))]
    public class CameraSystemEditor : Editor
    {
        #region members
            private SerializedProperty _cameraTargetField;
            private SerializedProperty _defaultCameraStateField;
            private SerializedProperty _useFixedUpdateField;
            private SerializedProperty _debugCurrentStateNameField;

            private Texture2D _logoTexture;

            private ReorderableList _userDefinedFlagsList;
        #endregion members

        #region methods
            private void OnEnable()
            {
                this._userDefinedFlagsList = new ReorderableList(serializedObject, serializedObject.FindProperty("_userDefinedFlags"), true, true, true, true);

                this._userDefinedFlagsList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = this._userDefinedFlagsList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("_name"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + rect.width - 15, rect.y, 20, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("_value"), GUIContent.none);
                };

                this._userDefinedFlagsList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "User Defined Flags");
                };
            }

            public override void OnInspectorGUI()
            {
                if (this._logoTexture == null)
                {
                    this._logoTexture = AssetDatabase.LoadAssetAtPath("Assets/StrayTech/Camera System/Graphics/Logo.png", typeof(Texture2D)) as Texture2D;
                }

                // fail gracefully if CacheFields throws an exception.
                if (this.CacheFields() == false)
                {
                    this.DrawCacheFieldsFailureMessage();
                    return;
                }

                DrawCustomGUI();
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
                    this._cameraTargetField = serializedObject.FindPropertyExplosive("_cameraTarget");
                    this._defaultCameraStateField = serializedObject.FindPropertyExplosive("_defaultCameraState");
                    this._useFixedUpdateField = serializedObject.FindPropertyExplosive("_useFixedUpdate");
                    this._debugCurrentStateNameField = serializedObject.FindPropertyExplosive("_debugCurrentStateName");
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
                EditorGUILayout.Space();

                Rect space = EditorGUILayout.BeginVertical();
                EditorGUILayout.TextArea(String.Empty, GUIStyle.none, GUILayout.Height(EditorGUIUtility.currentViewWidth * 0.25f));
                EditorGUILayout.EndVertical();
                EditorGUI.DrawPreviewTexture(space, this._logoTexture);
                
                EditorTools.DrawDivider(12.0f);

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(this._cameraTargetField);
                    EditorGUILayout.PropertyField(this._defaultCameraStateField);
                    EditorGUILayout.PropertyField(this._useFixedUpdateField);
                    EditorGUILayout.PropertyField(this._debugCurrentStateNameField);
                    EditorTools.DrawDivider(12.0f);
                    this._userDefinedFlagsList.DoLayoutList();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
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