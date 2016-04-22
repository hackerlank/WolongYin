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
        /// <para>Causes this property to be rendered with a "Copy to Clipboard" button next to it.</para>
        /// <para>The value put in the Clipboard is the output of the attached SerializedProperty's "ValueAsString" output.</para>
        /// </summary>
        public class CopyableAttribute : PropertyAttribute { }
    }
}