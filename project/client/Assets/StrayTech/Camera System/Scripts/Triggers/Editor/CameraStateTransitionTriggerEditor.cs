using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace StrayTech
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraStateTransitionTrigger))]
    public class CameraStateTransitionTriggerEditor : CameraSystemTriggerBaseEditor
    {
        #region members
            private SerializedProperty _targetCameraStateDefinitionField;
        #endregion members

        #region methods
            public override void OnInspectorGUI()
            {
                this._targetCameraStateDefinitionField = serializedObject.FindPropertyExplosive("_targetCameraStateDefinition");

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(this._targetCameraStateDefinitionField);
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
