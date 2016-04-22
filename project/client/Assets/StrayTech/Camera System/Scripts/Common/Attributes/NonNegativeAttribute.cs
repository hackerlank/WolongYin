using UnityEngine;
using System.Collections;

namespace StrayTech
{
    namespace CustomAttributes
    {
        /// <summary>
        /// Prevents the value from being set to negative in the inspector. 
        /// </summary>
        public class NonNegativeAttribute : PropertyAttribute { }
    }
}