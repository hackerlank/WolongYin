using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace StrayTech
{
    /// <summary>
    /// Will get initialized by Unity. Searches the codebase for any classes which have a RenderHierarchyIconAttribute attribute. 
    /// For each of those will attempt to render the icon specified by the attribute in the Unity editor.
    /// </summary>
    [InitializeOnLoad]
    public static class RenderHierarchyIconController
    {
        #region const members
            /// <summary>
            /// The default size we render Icons with. 
            /// </summary>
            public const float DEFAULT_ICON_SIZE = 18f;
            /// <summary>
            /// The amount to indent if we must stagger the icon. 
            /// </summary>
            public const float INDENT_SIZE = DEFAULT_ICON_SIZE * 1.25f;
        #endregion const members

        #region members
            /// <summary>
            /// Stores the Type mapped to the Loaded Texture2D asset of their icon. 
            /// </summary>
            private static Dictionary<Type, Texture2D> _loadedIcons = new Dictionary<Type, Texture2D>();
        #endregion members

        #region constructors
            static RenderHierarchyIconController()
            {
                FindAllAttributes();

                //No need to listen to updates if we couldn't load any scripts with this attribute! 
                if (_loadedIcons.Count == 0)
                    return;

                EditorApplication.hierarchyWindowItemOnGUI += RenderIcons;
            }
        #endregion construcors

        #region methods
            /// <summary>
            /// Search the Assembly for all scripts which have our attribute. 
            /// </summary>
            private static void FindAllAttributes()
            {
                var assembly = typeof(RenderHierarchyIconAttribute).Assembly;

                if (assembly == null)
                    return;

                var allTypesWithAttribute = from type in assembly.GetTypes()
                                            where Attribute.IsDefined(type, typeof(RenderHierarchyIconAttribute))
                                            select type;

                if (allTypesWithAttribute == null)
                    return;

                TryToLoadAllIcons(allTypesWithAttribute);
            }

            /// <summary>
            /// Iterate over all types we found which had the attribute and try to load up the icon specified. 
            /// </summary>
            /// <param name="allTypesWithAttribute"></param>
            private static void TryToLoadAllIcons(IEnumerable<Type> allTypesWithAttribute)
            {
                var previouslyLoadedIcons = new Dictionary<string, Texture2D>();

                foreach (var type in allTypesWithAttribute)
                {
                    //Get the RenderHierarchyIconAttribute that we expect to be present on this type. 
                    var iconAttribute = type.GetCustomAttribute<RenderHierarchyIconAttribute>(true);

                    if (iconAttribute == null)
                    {
                        Debug.LogErrorFormat("Failed to get a RenderHierarchyIconAttribute from type {0} which we expected to have one!", type.Name);
                        continue;
                    }

                    //Bail if someone messed up the attribute constructor by providing it bad data. 
                    if (string.IsNullOrEmpty(iconAttribute._iconAssetPath) == true)
                    {
                        Debug.LogErrorFormat("Found a RenderHierarchyIconAttribute for type {0} but there was an empty Icon asset path specified! Check Attribute Constructor!", type.Name);
                        continue;
                    }

                    //If we already loaded the texture specified by the string, then skip it. 
                    if (previouslyLoadedIcons.ContainsKey(iconAttribute._iconAssetPath) == true)
                    {
                        //Cache that this type uses this texture! 
                        _loadedIcons.Add(type, previouslyLoadedIcons[iconAttribute._iconAssetPath]);

                        continue;
                    }

                    //Attempt to load the Texture2D asset specified by this icon attribute. 
                    var foundIconAsset = AssetDatabase.LoadAssetAtPath(iconAttribute._iconAssetPath, typeof(Texture2D)) as Texture2D;

                    if (foundIconAsset == null)
                    {
                        Debug.LogErrorFormat("Attempted to load icon specified in RenderHierarchyIconAttribute for type {0}, but the asset could not be found! Check the path: {1}", type.Name, iconAttribute._iconAssetPath);
                    }

                    //Cache this load so we don't do any potential reloads. 
                    previouslyLoadedIcons.Add(iconAttribute._iconAssetPath, foundIconAsset);

                    //Cache that this type uses this texture! 
                    _loadedIcons.Add(type, foundIconAsset);
                }
            }

            /// <summary>
            /// Our hierarchyWindowItemOnGUI callback. We should see if the provided GO has any scripts which we should render for.. 
            /// </summary>
            private static void RenderIcons(int instanceID, Rect selectionRect)
            {
                //Get the gameobject that we should render icons for. 
                var instanceGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                //Bail if something wonky happened. 
                if (instanceGameObject == null)
                    return;

                //All of the icons we have rendered (prevent double rendering of Icons)
                var renderedIcons = new HashSet<Texture2D>();

                //Iterate over all the MonoBehaviours attached to this gameobject. 
                foreach (var component in instanceGameObject.GetComponents<MonoBehaviour>())
                {
                    // Null check for missing scripts
                    if (component == null)
                    {
                        continue;
                    }

                    //Waht type of component is this?
                    var typeOfComponent = component.GetType();

                    //Skip if this component is not one that we should render an Icon for. 
                    if (_loadedIcons.ContainsKey(typeOfComponent) == false)
                    {
                        continue;
                    }

                    //Skip if we have already rendered this Icon. 
                    if (renderedIcons.Contains(_loadedIcons[typeOfComponent]) == true)
                    {
                        continue;
                    }

                    //Render our Icon!! 
                    GUI.DrawTexture(EditorExtensions.ExtractSpaceHorizontal(ref selectionRect, DEFAULT_ICON_SIZE, false), _loadedIcons[typeOfComponent], ScaleMode.ScaleToFit, true);

                    //Cache that we have rendered this icon (prevent double rendering)
                    renderedIcons.Add(_loadedIcons[typeOfComponent]);
                }
            }
        #endregion methods
    }
}