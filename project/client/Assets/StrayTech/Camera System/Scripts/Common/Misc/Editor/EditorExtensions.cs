using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace StrayTech
{
    /// <summary>
    /// Any extension methods used by editor classes can be created here. 
    /// </summary>
    public static class EditorExtensions
    {
        #region const members
            public const float LINE_HEIGHT = 16f;
            public const float SPACING = 3f;
            public const float SPACED_LINE = LINE_HEIGHT + SPACING;
        #endregion const members

        #region static members
            /// <summary>
            /// A Blank GUIContent.
            /// </summary>
            public static GUIContent Blank = new GUIContent();
        #endregion static members

        #region methods
            /// <summary>
            /// "Extracts" a single line from the given source Rect, and returns the sub-Rect.
            /// </summary>
            /// <param name="canvas">The source area to extract space from. Space will be pulled from the top of the Rect.</param>
            /// <returns>A single-line high Rect, positioned at the top of the Canvas.</returns>
            public static Rect ExtractSpace(ref Rect canvas)
            {
                return EditorExtensions.ExtractSpace(ref canvas, EditorGUIUtility.singleLineHeight);
            }

            /// <summary>
            /// "Extracts" a Rect of the given height from the given source Rect, and returns the sub-Rect.
            /// </summary>
            /// <param name="canvas">The source area to extract space from. Space will be pulled from the top of the Rect.</param>
            /// <param name="height">The amount of space to extract from the Canvas.</param>
            /// <returns>A rect with height "height", positioned at the top of the Canvas.</returns>
            public static Rect ExtractSpace(ref Rect canvas, float height)
            {
                // if height is taller than the canvas, just extract the rest of the canvas.
                float amountToExtract = canvas.height >= height ? height : canvas.height;

                // create the extracted area
                Rect output = new Rect(canvas);
                output.height = amountToExtract;

                // shove the canvas's top down
                canvas.yMin += amountToExtract;

                return output;
            }

            public static Rect ExtractSpaceHorizontal(ref Rect canvas, float width, bool rightToLeft = true)
            {
                float amountToExtract = canvas.width >= width ? width : canvas.width;

                Rect output = new Rect(canvas);
                output.width = amountToExtract;

                if (rightToLeft == true)
                {
                    canvas.xMin += amountToExtract;
                }
                else
                {
                    canvas.xMax -= amountToExtract;
                    output.x = canvas.xMax;
                }

                return output;
            }

            public static Rect PadRectangle(this Rect source, float padding)
            {
                return PadRectangle(source, padding, padding, padding, padding);
            }

            public static Rect PadRectangle(this Rect source, float top, float left, float bottom, float right)
            {
                Rect output = new Rect(source);

                output.yMin += top;
                output.xMin += left;
                output.yMax -= bottom;
                output.xMax -= right;

                return output;
            }

            public static void DrawFrame(Rect canvas, float padding, Color border, Color inner)
            {
                EditorGUI.DrawRect(canvas, border);
                EditorGUI.DrawRect(EditorExtensions.PadRectangle(canvas, padding), inner);
            }

            public static string ValueAsString(this SerializedProperty property)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.AnimationCurve:
                        return property.animationCurveValue.ToString();
                    case SerializedPropertyType.ArraySize:
                        return property.ToString();
                    case SerializedPropertyType.Boolean:
                        return property.boolValue.ToString();
                    case SerializedPropertyType.Bounds:
                        return property.boundsValue.ToString();
                    case SerializedPropertyType.Character:
                        return property.stringValue.ToString();
                    case SerializedPropertyType.Color:
                        return property.colorValue.ToString();
                    case SerializedPropertyType.Enum:
                        return property.enumNames[property.enumValueIndex];
                    case SerializedPropertyType.Float:
                        return property.ToString();
                    case SerializedPropertyType.Generic:
                        return property.ToString();
                    case SerializedPropertyType.Gradient:
                        return property.ToString();
                    case SerializedPropertyType.Integer:
                        return property.intValue.ToString();
                    case SerializedPropertyType.LayerMask:
                        return property.ToString();
                    case SerializedPropertyType.ObjectReference:
                        return property.objectReferenceValue != null ? property.objectReferenceValue.name : "null";
                    case SerializedPropertyType.Quaternion:
                        return property.quaternionValue.ToString();
                    case SerializedPropertyType.Rect:
                        return property.rectValue.ToString();
                    case SerializedPropertyType.String:
                        return property.stringValue;
                    case SerializedPropertyType.Vector2:
                        return property.vector2Value.ToString();
                    case SerializedPropertyType.Vector3:
                        return property.vector3Value.ToString();
                    default:
                        break;
                }

                return string.Empty;
            }

            /// <summary>
            /// Find serialized property by name. Throws an exception if the property couldn't be found.
            /// </summary>
            /// <param name="source">This SerializedObject.</param>
            /// <param name="propertyPath">The name of the field/property to find.</param>
            /// <returns>The SerializedProperty with the given name.</returns>
            public static SerializedProperty FindPropertyExplosive(this SerializedObject source, string propertyPath)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                if (string.IsNullOrEmpty(propertyPath))
                {
                    throw new ArgumentException("propertyName");
                }

                var output = source.FindProperty(propertyPath);

                if (output == null)
                {
                    throw new MissingFieldException(string.Format("No such field '{0}' in '{1}'!", propertyPath, source.targetObject.name));
                }

                return output;
            }

            /// <summary>
            /// Find serialized property by name. Throws an exception if the property couldn't be found.
            /// </summary>
            /// <param name="source">This SerializedProperty.</param>
            /// <param name="relativePropertyPath">The name of the field/property to find.</param>
            /// <returns>The SerializedProperty with the given name.</returns>
            public static SerializedProperty FindPropertyRelativeExplosive(this SerializedProperty source, string relativePropertyPath)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                if (string.IsNullOrEmpty(relativePropertyPath))
                {
                    throw new ArgumentException("propertyName");
                }

                var output = source.FindPropertyRelative(relativePropertyPath);

                if (output == null)
                {
                    throw new MissingFieldException(string.Format("No such field '{0}' in '{1}'!", relativePropertyPath, source.name));
                }

                return output;
            }

            /// <summary>
            /// Niceifys the inspector name of a given property
            /// by stripping any leading underscores and capitalizing the first letter
            /// of the property. 
            /// </summary>
            /// <returns></returns>
            public static string NiceifyPropertyName(this string myString)
            {
                //Don't bother with 2 character names, they probably aren't savable anyway.
                if (myString.Length <= 2)
                    return myString;

                string toReturn = myString;

                try
                {
                    //Remove the leading _ if it exists. 
                    if (toReturn.StartsWith("_"))
                    {
                        toReturn = toReturn.Remove(0, 1);
                    }

                    //Replace the first character with the uppercase version of that character. 
                    var firstCharacter = toReturn[0];
                    toReturn = toReturn.Remove(0, 1);
                    toReturn = toReturn.Insert(0, char.ToUpper(firstCharacter).ToString());

                    //Add a space before each capital letter. 
                    toReturn = Regex.Replace(toReturn, "([a-z])([A-Z])", "$1 $2");
                }
                catch (System.Exception)
                {
                    //Catch any possible exceptions with our string operations and reset our result if anything goes wrong. 
                    toReturn = myString;
                }

                return toReturn;
            }

            /// <summary>
            /// Render a property on a new line. 
            /// </summary>
            public static Rect RenderSingleLineProperty(Rect currentPosition, SerializedProperty property, string overrideName = "")
            {
                if (property == null)
                    return currentPosition;

                //Update position for new line. 
                currentPosition = MoveToNewLine(currentPosition);

                string nameString = property.name;

                if (string.IsNullOrEmpty(overrideName) == false)
                {
                    nameString = overrideName;
                }

                //Render the property. . 
                EditorGUI.PropertyField(currentPosition, property, new GUIContent(ObjectNames.NicifyVariableName(nameString)));

                return currentPosition;
            }

            public static void RenderBackgroundRect(Rect position, float propertyHeight, string title)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    //Make the rect take up the entire property. 
                    Rect newPosition = new Rect(position.xMin, position.yMin, position.width, Mathf.Max(0, propertyHeight));

                    GUI.skin.box.normal.textColor = GUI.skin.label.normal.textColor;
                    GUI.skin.box.fontStyle = FontStyle.BoldAndItalic;

                    if (string.IsNullOrEmpty(title) == false)
                    {
                        GUI.skin.box.Draw(newPosition, new GUIContent(ObjectNames.NicifyVariableName(title)), 0);
                    }
                    else
                    {
                        GUI.skin.box.Draw(newPosition, GUIContent.none, 0);
                    }
                }
            }

            /// <summary>
            /// Move the position to a new line. 
            /// </summary>
            public static Rect MoveToNewLine(Rect position)
            {
                return new Rect(position.xMin, position.yMin + (LINE_HEIGHT + SPACING), position.width - SPACING, LINE_HEIGHT);
            }

            /// <summary>
            /// Calculate the total height of a property based on the number of single line elements. 
            /// </summary>
            public static float CalculatePropertyHeight(float numberOfSingleLineElements)
            {
                if (numberOfSingleLineElements <= 0)
                    return LINE_HEIGHT;

                return (LINE_HEIGHT * numberOfSingleLineElements) + ((numberOfSingleLineElements + 1) * SPACING);
            }

            /// <summary>
            /// Remove all interfaces of the provided type from the provided GameObject. 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="gameObject"></param>
            public static void RemoveInterfaces<T>(GameObject gameObject)
                where T : class
            {
                //Bail if provided null argument.
                if (gameObject == null)
                    return;

                //Bail if provided incorrect type. 
                if (typeof(T).IsInterface == false)
                    return;

                //Get all attached interfaces of the provided type. 
                var foundInterfaces = gameObject.GetInterfaces<T>();

                for (int i = foundInterfaces.Length - 1; i >= 0; i--)
                {
                    //We can cast the interface to a component because we know that only 
                    //interfaces which are also monobehaviors will be found by GetInterfaces. 
                    var component = foundInterfaces[i] as Component;

                    //Call DestroyImmediate because we are in editor mode. 
                    UnityEngine.Object.DestroyImmediate(component);
                }
            }

            /// <summary>
            /// Remove all MonoBehaviors of the provided type from the provided GameObject. 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="gameObject"></param>
            public static void RemoveBehaviors<T>(GameObject gameObject)
                where T : MonoBehaviour
            {
                //Bail if provided null argument.
                if (gameObject == null)
                    return;

                //Get all attached interfaces of the provided type. 
                var foundComponents = gameObject.GetComponents<T>();

                for (int i = foundComponents.Length - 1; i >= 0; i--)
                {
                    //Call DestroyImmediate because we are in editor mode. 
                    UnityEngine.Object.DestroyImmediate(foundComponents[i]);
                }
            }

            /// <summary>
            /// Helper method which attempts to
            /// 1. Load the provided asset in the resources folder. 
            /// 2. Get a MonoBehaviour Component of type T from the loaded asset
            /// </summary>
            /// <returns></returns>
            public static T GetComponentFromLoadedResource<T>(string resourcePath)
                where T : Component
            {
                if (string.IsNullOrEmpty(resourcePath))
                    return null;

                //Try to load and instantiate the resource specified at the asset path. 
                var loadedResource = GameObject.Instantiate(Resources.Load<GameObject>(resourcePath)) as GameObject;

                if (loadedResource == null)
                {
                    Debug.LogError("Could not load resource at asset path: " + resourcePath);
                    //Be safe and destroy immediate.. the Instantiate ~*could*~ have created something (if the 
                    //asset it loaded was not a GameObject the "as" check would have made loadedResource null, however something 
                    //definitely was loaded). So just call this, passing in null wont explode so it can't hurt. 
                    GameObject.DestroyImmediate(loadedResource);
                    return null;
                }

                //Attempt to get the desired component. 
                var foundComponent = loadedResource.GetComponent<T>();

                if (foundComponent == null)
                {
                    //bail if spawned prefab didn't have the requested component. 
                    Debug.LogError(string.Format("Could not get a {0} component from asset loaded from path {1}. Destroying!", typeof(T).Name, resourcePath));
                    GameObject.DestroyImmediate(loadedResource);
                    return null;
                }

                return foundComponent;
            }

            public static T LoadScriptableObject<T>(string resourcePath)
                where T : ScriptableObject
            {
                if (string.IsNullOrEmpty(resourcePath))
                    return null;

                //Try to load the resource specified at the asset path. 
                var loadedResource = Resources.Load(resourcePath) as T;

                if (loadedResource == null)
                {
                    Debug.LogError("Could not load resource at asset path: " + resourcePath);
                    GameObject.DestroyImmediate(loadedResource);
                    return null;
                }

                return loadedResource;
            }


            /// <summary>
            /// Show the user the new object they created using our menu. 
            /// </summary>
            /// <param name="toFocus"></param>
            public static void FocusOnGameObject(GameObject toFocus)
            {
                if (toFocus == null)
                    return;

                //Make the new object the active selection and move the camera to focus on it. 
                Selection.activeGameObject = toFocus;
                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                EditorGUIUtility.PingObject(toFocus);
            }

            /// <summary>
            /// Returns the number of children of the provided parent, whose name matches
            /// the provided string. 
            /// </summary>
            public static int GetChildCountOfNamesContain(GameObject parent, string searchString)
            {
                if (parent == null || string.IsNullOrEmpty(searchString))
                    return 0;

                int foundChildrenCount = 0;

                foreach (Transform child in parent.transform)
                {
                    if (child.name.Contains(searchString))
                    {
                        foundChildrenCount++;
                    }
                }

                return foundChildrenCount;
            }
        #endregion methods
    }
}