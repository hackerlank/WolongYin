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
    [CustomPropertyDrawer(typeof(BelowAttribute))]
    public class BelowAttributePropertyDrawer : PropertyDrawer
    {
        #region members
            /// <summary>
            /// The BelowAttribute we are rendering. 
            /// </summary>
            private BelowAttribute _inspected;
        #endregion members

        #region methods
            /// <summary>
            /// Render our property drawer. 
            /// </summary>
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                _inspected = this.attribute as BelowAttribute;

                if (_inspected == null)
                {
                    Debug.LogException(new InvalidOperationException("BelowAttributePropertyDrawer rendering something that isn't a BelowAttribute!"));
                    return;
                }

                this.DrawProperty(position, property, label);
            }

            /// <summary>
            /// Render the GUI based on the type of the serialized property. 
            /// </summary>
            private void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.BeginChangeCheck();

                    var value = EditorGUI.FloatField(position, label, property.floatValue);

                    if (value >= _inspected.Max)
                    {
                        value = _inspected.Max - float.Epsilon;
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

                    if (value >= _inspected.Max)
                    {
                        value = Mathf.FloorToInt(_inspected.Max) - 1;
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
            }
        #endregion methods
    }
}