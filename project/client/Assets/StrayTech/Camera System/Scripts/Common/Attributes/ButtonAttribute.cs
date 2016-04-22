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
        /// Causes a boolean field to be rendered as a toggleable button.
        /// </summary>
        public class ButtonAttribute : PropertyAttribute
        {
            #region members
                /// <summary>
                /// The label to show in front of the button.
                /// </summary>
                public readonly string ButtonLabel = string.Empty;

                /// <summary>
                /// The text to show on the button.
                /// </summary>
                public readonly string ButtonText = string.Empty;
            #endregion members

            #region constructors
                /// <summary>
                /// Causes this field to be rendered as a button.
                /// </summary>
                /// <param name="buttonText">The text to display on the button.</param>
                /// <param name="buttonLabel">The label to display in front of the button.</param>
                public ButtonAttribute(string buttonText, string buttonLabel)
                {
                    this.ButtonText = buttonText;
                    this.ButtonLabel = buttonLabel;
                }

                /// <summary>
                /// Causes this field to be rendered as a button.
                /// </summary>
                /// <param name="buttonText">The text to display on the button.</param>
                public ButtonAttribute(string buttonText)
                {
                    this.ButtonText = buttonText;
                }

                /// <summary>
                /// Causes this field to be rendered as a button, with the field name being the button text.
                /// </summary>
                public ButtonAttribute() { }
            #endregion constructors
        }
    }
}