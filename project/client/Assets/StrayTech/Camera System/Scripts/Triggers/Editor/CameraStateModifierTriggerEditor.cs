using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace StrayTech
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraStateModifierTrigger))]
    public class CameraStateModifierTriggerEditor : CameraSystemTriggerBaseEditor
    {
        #region members
            private SerializedProperty _cameraStateModifierTargetField;
            private SerializedProperty _ignoreTriggerExitField;
        #endregion members

        #region methods
            public override void OnInspectorGUI()
            {
                this._cameraStateModifierTargetField = serializedObject.FindPropertyExplosive("_cameraStateModifierTarget");
                this._ignoreTriggerExitField = serializedObject.FindPropertyExplosive("_ignoreTriggerExit");

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(this._cameraStateModifierTargetField);
                    EditorGUILayout.PropertyField(this._ignoreTriggerExitField);
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
