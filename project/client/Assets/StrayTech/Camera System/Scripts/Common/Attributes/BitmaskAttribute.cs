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
        /// Causes an enum to be rendered as a multi-selectable field in the Inspector.
        /// </summary>
        public sealed class BitmaskAttribute : PropertyAttribute { }
    }
}