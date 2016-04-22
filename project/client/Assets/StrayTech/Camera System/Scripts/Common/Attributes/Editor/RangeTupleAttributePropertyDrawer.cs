using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    [CustomPropertyDrawer(typeof(RangeTupleAttribute))]
    public class RangeTupleAttributePropertyDrawer : PropertyDrawer
    {
        #region members
            private float? _lineHeight;

            private GUIStyle _quickLabelStyle;

            private RangeTupleAttribute _rangeData;
        #endregion members

        #region constructors
            private void Setup(SerializedProperty property, GUIContent label)
            {
                if (this._quickLabelStyle == null)
                {
                    this._quickLabelStyle = new GUIStyle("Label");
                }

                if (this._lineHeight.HasValue == false)
                {
                    this._lineHeight = base.GetPropertyHeight(property, label);
                }

                if (this._rangeData == null)
                {
                    this._rangeData = this.attribute as RangeTupleAttribute;
                }
            }
        #endregion constructors

        #region methods
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                this.Setup(property, label);

                if (this._rangeData.AllArgsValid == false || property.propertyType != SerializedPropertyType.Vector2)
                {
                    return this._lineHeight.Value;
                }
                else
                {
                    return this._lineHeight.Value * 2;
                }
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                this.Setup(property, label);

                #region validate
                    if (this._rangeData == null)
                    {
                        EditorGUI.LabelField(position, label, new GUIContent("How is this happening"));
                        return;
                    }

                    if (property.propertyType != SerializedPropertyType.Vector2)
                    {
                        EditorGUI.LabelField(position, label, new GUIContent("RangeTuppleAttribute can only be applied to fields of type Vector2!"));
                        return;
                    }
                    if (this._rangeData.AllArgsValid == false)
                    {
                        EditorGUI.LabelField(position, label, new GUIContent("Invalid Min/Max values specified!"));
                        return;
                    }
                #endregion validate

                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

                Vector2 value = property.vector2Value;

                EditorGUI.BeginChangeCheck();
                {
                    position = EditorGUI.PrefixLabel(position, label);
                    EditorGUI.MinMaxSlider(EditorExtensions.ExtractSpace(ref position, this._lineHeight.Value), ref value.x, ref value.y, this._rangeData.Min, this._rangeData.Max);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (this._rangeData.ConstrainToIntegralValues)
                        {
                            value.x = Mathf.Floor(value.x);
                            value.y = Mathf.Floor(value.y);
                        }
                        property.vector2Value = value;
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    // lower bound
                    this._quickLabelStyle.alignment = TextAnchor.MiddleLeft;
                    EditorGUI.LabelField(position, this._rangeData.Min.ToString(), this._quickLabelStyle);

                    // actual values
                    this._quickLabelStyle.alignment = TextAnchor.MiddleCenter;
                    EditorGUI.LabelField(position, this.GetValueText(value), this._quickLabelStyle);

                    // upper bound
                    this._quickLabelStyle.alignment = TextAnchor.MiddleRight;
                    EditorGUI.LabelField(position, this._rangeData.Max.ToString(), this._quickLabelStyle);
                }

                EditorGUI.showMixedValue = false;
            }

            private string GetValueText(Vector2 value)
            {
                if (EditorGUI.showMixedValue == true)
                {
                    return "--";
                }

                if (Mathf.Approximately(value.x, value.y))
                {
                    return value.x.ToString();
                }

                return string.Format("{0} - {1}", value.x, value.y);
            }
        #endregion methods
    }
}