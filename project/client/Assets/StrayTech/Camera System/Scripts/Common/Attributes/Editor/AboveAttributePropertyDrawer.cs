using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEditor;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    /// <summary>
    /// A custom property drawer for AboveAttribute. 
    /// </summary>
    [CustomPropertyDrawer(typeof(AboveAttribute))]
    public class AboveAttributePropertyDrawer : PropertyDrawer
    {
        #region members
            /// <summary>
            /// The AboveAttribute we are rendering. 
            /// </summary>
            private AboveAttribute _inspected;
        #endregion members

        #region methods
            /// <summary>
            /// Render our property drawer. 
            /// </summary>
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                _inspected = this.attribute as AboveAttribute;

                if (_inspected == null)
                {
                    Debug.LogException(new InvalidOperationException("AboveAttributePropertyDrawer rendering something that isn't a AboveAttribute!"));
                    return;
                }

                this.DrawProperty(position, property, label);
            }

            /// <summary>
            /// Render the GUI based on the type of the serialized property. 
            /// </summary>
            private void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

                if (property.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.BeginChangeCheck();

                    var value = EditorGUI.FloatField(position, label, property.floatValue);

                    if (value <= _inspected.Min)
                    {
                        value = _inspected.Min + float.Epsilon;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.floatValue = value;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.BeginChangeCheck();

                    var value = EditorGUI.IntField(position, label, property.intValue);

                    if (value <= _inspected.Min)
                    {
                        value = Mathf.FloorToInt(_inspected.Min + 1);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.intValue = value;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {
                    base.OnGUI(position, property, label);
                }

                EditorGUI.showMixedValue = false;
            }
        #endregion methods
    }
}