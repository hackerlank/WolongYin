using UnityEngine;
using System.Collections;
using System;

namespace StrayTech
{
    /// <summary>
    /// The attribute which allows us to specify the Icon which will be rendered on this GameObject in the Unity Editor Hierarchy. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RenderHierarchyIconAttribute : Attribute
    {
        #region members
            /// <summary>
            /// The asset path to the icon which was specified in the Attribute constructor. 
            /// </summary>
            public readonly string _iconAssetPath;
        #endregion members

        #region constructors
            public RenderHierarchyIconAttribute(string iconAssetPath)
            {
                this._iconAssetPath = iconAssetPath;
            }
        #endregion construcors
    }
}