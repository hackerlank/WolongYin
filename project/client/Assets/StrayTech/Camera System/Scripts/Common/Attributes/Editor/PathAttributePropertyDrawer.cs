using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using StrayTech.CustomAttributes;

namespace StrayTech
{
    /// <summary>
    /// 
    /// </summary>
    [CustomPropertyDrawer(typeof(PathAttribute))]
    public class PathAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pathAttribute = this.attribute as PathAttribute;

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, "PathAttribute can only be used on fields of type 'String'.");
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);

            var contentCanvas = EditorGUI.PrefixLabel(position, label);

            if (GUI.Button(EditorExtensions.ExtractSpaceHorizontal(ref contentCanvas, 24.0f, false), "..."))
            {
                string dialogResult = string.Empty;

                EditorGUIUtility.editingTextField = false;

                switch (pathAttribute.PathType)
                {
                    case PathAttribute.SelectionType.Folder:
                        {
                            dialogResult = EditorUtility.OpenFolderPanel("Select Folder", property.stringValue, string.Empty);
                        }
                        break;
                    case PathAttribute.SelectionType.File:
                        {
                            dialogResult = EditorUtility.OpenFilePanel("Select File", property.stringValue, pathAttribute.FileExtension);
                        }
                        break;
                    default:
                        {
                            EditorUtility.DisplayDialog("Unknown PathType!", string.Format("The {0} for the field {1} had an unknown {2}: {3}.", typeof(PathAttribute).Name, property.name, typeof(PathAttribute.SelectionType).Name, pathAttribute.PathType.ToString()), "Ok");
                            return;
                        }
                }

                if (string.IsNullOrEmpty(dialogResult) == false)
                {
                    if (pathAttribute.RelativeToAssetsFolder == true)
                    {
                        var projectRoot = new Uri(Application.dataPath);
                        var selectedPath = new Uri(dialogResult);

                        dialogResult = projectRoot.MakeRelativeUri(selectedPath).ToString();
                    }

                    property.stringValue = dialogResult;
                }
            }

            property.stringValue = EditorGUI.TextField(contentCanvas, property.stringValue);

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}