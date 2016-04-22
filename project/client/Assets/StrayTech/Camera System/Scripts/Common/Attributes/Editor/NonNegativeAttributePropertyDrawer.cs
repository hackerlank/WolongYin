using UnityEngine;
using UnityEditor;
using System.Collections;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    /// <summary>
    /// 
    /// </summary>
    [CustomPropertyDrawer(typeof(NonNegativeAttribute))]
    public class NonNegativeAttributePropertyDrawer : PropertyDrawer
    {
        #region methods
            /// <summary>
            /// Prevent the value from being changed to negative. 
            /// </summary>
            /// <param name="position"></param>
            /// <param name="property"></param>
            /// <param name="label"></param>
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

                if (property.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.BeginChangeCheck();

                    var value = EditorGUI.FloatField(position, property.name.NiceifyPropertyName(), property.floatValue);

                    if (value < 0)
                    {
                        value = 0;
                    }

                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        property.floatValue = value;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.BeginChangeCheck();

                    var value = EditorGUI.IntField(position, property.name.NiceifyPropertyName(), property.intValue);

                    if (value < 0)
                    {
                        value = 0;
                    }

                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        property.intValue = value;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {
                    //Did not specify attribute over a float or int member.. 
                    base.OnGUI(position, property, label);
                }

                EditorGUI.showMixedValue = false;
            }
        #endregion methods
    }
}