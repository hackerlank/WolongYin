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
    [CustomPropertyDrawer(typeof(Helpbox))]
    public class HelpboxAttributePropertyDrawer : PropertyDrawer
    {
        #region members
            /// <summary>
            /// The AboveAttribute we are rendering. 
            /// </summary>
            private Helpbox _inspected;
        #endregion members

        #region methods
            /// <summary>
            /// Render our property drawer. 
            /// </summary>
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                _inspected = this.attribute as Helpbox;

                if (_inspected.IsNullOrInvalid())
                {
                    Debug.LogException(new InvalidOperationException("HelpboxAttributePropertyDrawer failed to render!"));
                    return;
                }

                EditorGUI.BeginChangeCheck();
                {
                    position = new Rect(position.xMin, position.yMin + EditorExtensions.SPACING, position.width, position.height - EditorExtensions.SPACING);

                    EditorGUI.HelpBox(position, _inspected.Message, GetDisplayType());
                }
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            /// <summary>
            /// Stupid dumb method. 
            /// </summary>
            /// <returns></returns>
            private MessageType GetDisplayType()
            {
                var helpBoxType = _inspected.MessageType;

                switch (helpBoxType)
                {
                    case Helpbox.Type.Info:
                        return MessageType.Info;
                    case Helpbox.Type.Warning:
                        return MessageType.Warning;
                    case Helpbox.Type.Error:
                        return MessageType.Error;
                    default:
                        return MessageType.None;
                }
            }

            /// <summary>
            /// Get prop height based on the message. 
            /// </summary>
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (_inspected.IsNullOrInvalid())
                {
                    return base.GetPropertyHeight(property, label);
                }

                //Let unity calcluate the text size. 
                var sizeOfText = GUI.skin.textField.CalcSize(new GUIContent(_inspected.Message));

                //this works somehow. 
                return (sizeOfText.x / sizeOfText.y) + EditorExtensions.SPACING * 2;
            }
        #endregion methods
    }
}