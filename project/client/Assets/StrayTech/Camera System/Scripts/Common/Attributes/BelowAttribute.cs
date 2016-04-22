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
        /// A CustomAttribute which forces a float or int value to be below a specified maximum value. 
        /// </summary>
        public class BelowAttribute : PropertyAttribute
        {
            #region members
                /// <summary>
                /// The max value that the targets value must be below. 
                /// </summary>
                public readonly float Max;
            #endregion members

            #region constructors
                public BelowAttribute(float maximumValue)
                {
                    this.Max = maximumValue;
                }
            #endregion construcors
        }
    }
}