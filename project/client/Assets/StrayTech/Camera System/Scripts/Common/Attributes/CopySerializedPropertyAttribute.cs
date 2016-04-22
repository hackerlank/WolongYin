using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    namespace CustomAttributes
    {
        /// <summary>
        /// A CustomAttribute which forces a float or int value to be above the specified minimum value. 
        /// </summary>
        public class CopySerializedPropertyAttribute : PropertyAttribute
        {
            #region members
                /// <summary>
                /// The path to the source property, with each getrelative seperated by the specified seperator. 
                /// </summary>
                public readonly string SourcePropertyPath;
                /// <summary>
                /// The path to the target property, with each getrelative seperated by the specified seperator. 
                /// </summary>
                public readonly string TargetPropertyPath;
            #endregion members

            #region constructors
                public CopySerializedPropertyAttribute(string sourcePropertyPath, string targetPropertyPath)
                {
                    this.SourcePropertyPath = sourcePropertyPath;
                    this.TargetPropertyPath = targetPropertyPath;
                }
            #endregion construcors
        }
    }
}