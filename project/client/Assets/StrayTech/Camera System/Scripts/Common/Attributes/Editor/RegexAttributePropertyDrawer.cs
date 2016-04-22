using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    [CustomPropertyDrawer(typeof(RegexAttribute))]
    public class RegexAttributePropertyDrawer : PropertyDrawer
    {
        #region members
            private Dictionary<string, string> _invalidEntriesCache = new Dictionary<string, string>();
        #endregion members

        #region methods
            public override void OnGUI(Rect canvas, SerializedProperty property, GUIContent label)
            {
                // render an error if we're applied to something that's not a string
                if (property.propertyType != SerializedPropertyType.String)
                {
                    GUIColorHelper.DoWithColor(Color.red,
                        () =>
                        {
                            EditorGUI.LabelField(canvas, label.text, "[Regex] should only be applied to string fields!");
                        }
                    );

                    return;
                }

                var regexAttribute = this.attribute as RegexAttribute;

                // crap out now if we have a bad pattern.
                if (RegexAttributePropertyDrawer.IsValidRegularExpression(regexAttribute.Pattern) == false)
                {
                    GUIColorHelper.DoWithColor(Color.red,
                        () =>
                        {
                            EditorGUI.LabelField(canvas, label.text, "Invalid pattern!");
                        }
                    );

                    return;
                }

                switch (regexAttribute.InputMode)
                {
                    case RegexAttribute.Mode.Force:
                        this.DrawForcedField(canvas, property, label, regexAttribute);
                        break;
                    case RegexAttribute.Mode.DisplayInvalid:
                        this.DrawDisplayInvalidField(canvas, property, label, regexAttribute);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// Draws a text entry field which allows text that doesn't match the attribute's pattern.
            /// </summary>
            private void DrawDisplayInvalidField(Rect canvas, SerializedProperty property, GUIContent label, RegexAttribute attribute)
            {
                Color backgroundResetColor = GUI.backgroundColor;
                Color displayColor = backgroundResetColor;
                string displayText = property.stringValue;

                // check to see if we have a record of an invalid entry for this property
                bool inputMatchesPattern = this._invalidEntriesCache.ContainsKey(property.propertyPath) == false;

                // if we do, switch the display to lookin' like invalid mode
                if (inputMatchesPattern == false)
                {
                    displayColor = Color.red;

                    // and change the display text to whatever that bad value is
                    displayText = this._invalidEntriesCache[property.propertyPath];

                    label = new GUIContent(label);
                    label.tooltip = string.Format("This field failed to match the regular expression '{0}'. It's value will not be saved when you navigate away from this inspector!", attribute.Pattern);
                }

                GUI.backgroundColor = displayColor;

                EditorGUI.BeginChangeCheck();

                string newValue = EditorGUI.TextField(canvas, label, displayText);

                if (EditorGUI.EndChangeCheck())
                {
                    if (Regex.IsMatch(newValue, attribute.Pattern, attribute.MatchOptions) == false)
                    {
                        this._invalidEntriesCache.AddOrSet(property.propertyPath, newValue);
                    }
                    else
                    {
                        this._invalidEntriesCache.Remove(property.propertyPath);

                        property.stringValue = newValue;

                        property.serializedObject.ApplyModifiedProperties();
                    }
                }

                GUI.backgroundColor = backgroundResetColor;
            }

            /// <summary>
            /// Draws a text entry field where text is immediately discarded if it doesn't match the attribute's pattern.
            /// </summary>
            private void DrawForcedField(Rect canvas, SerializedProperty property, GUIContent label, RegexAttribute attribute)
            {
                EditorGUI.BeginChangeCheck();

                string newValue = EditorGUI.TextField(canvas, label, property.stringValue);

                if (EditorGUI.EndChangeCheck())
                {
                    if (Regex.IsMatch(newValue, attribute.Pattern, attribute.MatchOptions) == false)
                    {
                        return;
                    }

                    property.stringValue = newValue;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            private static bool IsValidRegularExpression(string pattern)
            {
                // thanks Jeff Atwood!
                // http://stackoverflow.com/a/1775017/831796

                if (string.IsNullOrEmpty(pattern)) return false;

                try
                {
                    Regex.Match("", pattern);
                }
                catch (ArgumentException)
                {
                    return false;
                }

                return true;
            }
        #endregion methods
    }
}