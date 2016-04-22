using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    [CustomPropertyDrawer(typeof(CustomAttributes.BitmaskAttribute))]
    public class BitmaskAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            EditorGUI.BeginProperty(position, GUIContent.none, property);
            {
                int newValue = default(int);
                EditorGUI.BeginChangeCheck();
                {
                    newValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = newValue;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUI.EndProperty();

            EditorGUI.showMixedValue = false;
        }
    }
}