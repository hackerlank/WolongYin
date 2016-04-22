using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace StrayTech
{
    [CustomEditor(typeof(CameraSystemTriggerBase))]
    public class CameraSystemTriggerBaseEditor : Editor
    {
        #region members
            private SerializedProperty _triggerColliderTypeField;
            private SerializedProperty _meshField;
            private SerializedProperty _volumeColorField;
            private SerializedProperty _renderSolidVolumeField;
            private SerializedProperty _renderOnlyWhenSelectedField;
            private SerializedProperty _tagFilterField;
            private SerializedProperty _layerMaskField;
            private SerializedProperty _singleUseTriggerField;
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
                    this._triggerColliderTypeField = serializedObject.FindPropertyExplosive("_triggerColliderType");
                    this._meshField = serializedObject.FindPropertyExplosive("_mesh");
                    this._volumeColorField = serializedObject.FindPropertyExplosive("_volumeColor");
                    this._renderSolidVolumeField = serializedObject.FindPropertyExplosive("_renderSolidVolume");
                    this._renderOnlyWhenSelectedField = serializedObject.FindPropertyExplosive("_renderOnlyWhenSelected");
                    this._tagFilterField = serializedObject.FindPropertyExplosive("_tagFilter");
                    this._layerMaskField = serializedObject.FindPropertyExplosive("_layerMask");
                    this._singleUseTriggerField = serializedObject.FindPropertyExplosive("_singleUseTrigger");
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
                EditorGUILayout.PropertyField(this._triggerColliderTypeField);

                if ((CameraSystemTriggerBase.TriggerColliderType)this._triggerColliderTypeField.enumValueIndex == CameraSystemTriggerBase.TriggerColliderType.ConvexMesh)
                {
                    EditorGUILayout.PropertyField(this._meshField);
                }
                EditorGUILayout.PropertyField(this._volumeColorField);
                EditorGUILayout.PropertyField(this._renderSolidVolumeField);
                EditorGUILayout.PropertyField(this._renderOnlyWhenSelectedField);
                EditorTools.DrawDivider(6.0f);
                EditorGUILayout.PropertyField(this._tagFilterField);
                EditorGUILayout.PropertyField(this._layerMaskField);
                EditorGUILayout.PropertyField(this._singleUseTriggerField);
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