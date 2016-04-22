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
        public class AboveAttribute : PropertyAttribute
        {
            #region members
                /// <summary>
                /// The minimum value that the targets value must be above. 
                /// </summary>
                public readonly float Min;
            #endregion members

            #region constructors
                public AboveAttribute(float minimumValue)
                {
                    this.Min = minimumValue;
                }
            #endregion construcors
        }
    }
}