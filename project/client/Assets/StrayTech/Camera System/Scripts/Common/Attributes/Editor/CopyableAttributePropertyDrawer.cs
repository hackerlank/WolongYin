using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEditor;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    [CustomPropertyDrawer(typeof(CopyableAttribute))]
    public class CopyableAttributePropertyDrawer : PropertyDrawer
    {
        private Texture _clipboard;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (this._clipboard == null)
            {
                this._clipboard = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("cb0c76a45c0e1f64788be1f278d1d65f"), typeof(Texture)) as Texture;
            }

            Rect copyButtonCanvas = EditorExtensions.ExtractSpaceHorizontal(ref position, 20.0f);

            EditorGUI.PropertyField(position, property, label);

            if (GUI.Button(copyButtonCanvas, _clipboard))
            {
                EditorGUIUtility.systemCopyBuffer = property.ValueAsString();
                EditorWindow.focusedWindow.ShowNotification(new GUIContent(string.Format("\"{0}\" copied to clipboard.", EditorGUIUtility.systemCopyBuffer)));
            }
        }
    }
}