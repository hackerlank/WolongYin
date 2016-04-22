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
    /// A custom property drawer for CopySerializedPropertyAttribute. 
    /// </summary>
    [CustomPropertyDrawer(typeof(CopySerializedPropertyAttribute))]
    public class CopySerializedPropertyAttributeEditor : PropertyDrawer
    {
        #region members
            /// <summary>
            /// The CopySerializedPropertyAttribute we are rendering. 
            /// </summary>
            private CopySerializedPropertyAttribute _inspected;
            /// <summary>
            /// The cached Object that was dropped onto our copy field. 
            /// </summary>
            private UnityEngine.Object _droppedSourceObject;
        #endregion members

        #region methods
            /// <summary>
            /// Render our property drawer. 
            /// </summary>
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                _inspected = this.attribute as CopySerializedPropertyAttribute;

                if (_inspected == null)
                {
                    Debug.LogException(new InvalidOperationException("CopyPropertiesAttribute rendering something that isn't a CopyPropertiesAttribute!"));
                    return;
                }

                EditorGUI.BeginChangeCheck();
                {
                    _droppedSourceObject = EditorGUI.ObjectField(position, "Copy From", _droppedSourceObject, typeof(UnityEngine.Object), true);
                }
                if (EditorGUI.EndChangeCheck() == true)
                {
                    if (TryToCopyProperties(property) == false)
                    {
                        //Clear the drop target reference. 
                        _droppedSourceObject = null;

                        return;
                    }

                    //Clear the drop target reference. 
                    _droppedSourceObject = null;

                    //Save the changes to ourself. 
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            /// <summary>
            /// Attempt to copy the source property into the target property. 
            /// </summary>
            private bool TryToCopyProperties(SerializedProperty property)
            {
                if (_droppedSourceObject == null)
                    return false;

                //The found property on the drop target source object. 
                SerializedProperty sourceProperty = null;

                //If the source is a gameobject then we need to search all monos on it. 
                var sourceAsGameObject = _droppedSourceObject as GameObject;


                if (sourceAsGameObject != null)
                {
                    //If source was a GameObject we have to search all monos. 
                    sourceProperty = TryToFindSourceFromGameObject(sourceAsGameObject);
                }
                else
                {
                    //Create a new serialized object from the drop target. 
                    var sourceSerializedObject = new SerializedObject(_droppedSourceObject);

                    //Try to find the source property from the target serialized object. 
                    sourceProperty = sourceSerializedObject.FindProperty(_inspected.SourcePropertyPath);
                }

                //Bail if we couldn't find the property. 
                if (sourceProperty == null)
                {
                    Debug.LogError("Failed to find serialized property matching path: " + _inspected.SourcePropertyPath + " on target: " + _droppedSourceObject.name);
                    return false;
                }

                //Copy the source property values into our target. 
                property.serializedObject.CopyFromSerializedProperty(sourceProperty);

                return true;
            }

            /// <summary>
            /// Search all monobehaviors on the source instead of searching on the source itself. 
            /// </summary>
            private SerializedProperty TryToFindSourceFromGameObject(GameObject sourceAsGameObject)
            {
                var allMonos = sourceAsGameObject.GetComponents<MonoBehaviour>();

                foreach (var mono in allMonos)
                {
                    //Create a new serialized object from this mono. 
                    var sourceSerializedObject = new SerializedObject(mono);

                    //Try to find the source property from the target serialized object. 
                    var foundProperty = sourceSerializedObject.FindProperty(_inspected.SourcePropertyPath);

                    if (foundProperty != null)
                    {
                        return foundProperty;
                    }
                }

                return null;
            }
        #endregion methods
    }
}