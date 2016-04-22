using UnityEditor;
using UnityEngine;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var buttonAttribute = (this.fieldInfo.GetCustomAttribute<ButtonAttribute>());

            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, "ButtonAttribute can only be used on fields of type 'Boolean'.");
                return;
            }

            if (string.IsNullOrEmpty(buttonAttribute.ButtonText) == false)
            {
                label.text = buttonAttribute.ButtonText;
            }

            if (string.IsNullOrEmpty(buttonAttribute.ButtonLabel) == true)
            {
                property.boolValue = GUI.Toggle(position, property.boolValue, label, "Button");
            }


            property.serializedObject.ApplyModifiedProperties();
        }
    }
}