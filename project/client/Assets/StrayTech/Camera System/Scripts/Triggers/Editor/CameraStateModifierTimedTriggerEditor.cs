using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace StrayTech
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraStateModifierTimedTrigger))]
    public class CameraStateModifierTimedTriggerEditor : CameraSystemTriggerBaseEditor
    {
        #region members
            private SerializedProperty _cameraStateModifierTargetField;
            private SerializedProperty _enabledDurationField;
        #endregion members

        #region methods
            public override void OnInspectorGUI()
            {
                this._cameraStateModifierTargetField = serializedObject.FindPropertyExplosive("_cameraStateModifierTarget");
                this._enabledDurationField = serializedObject.FindPropertyExplosive("_enabledDuration");

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(this._cameraStateModifierTargetField);
                    EditorGUILayout.PropertyField(this._enabledDurationField);
                    EditorTools.DrawDivider(6.0f);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                }

                base.OnInspectorGUI();
            }
        #endregion methods
    }
}
