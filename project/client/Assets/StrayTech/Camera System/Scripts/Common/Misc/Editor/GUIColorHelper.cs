using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace StrayTech
{
    /// <summary>
    /// Facilitates temporarily changing GUI.Color.
    /// </summary>
    public class GUIColorHelper : IDisposable
    {
        #region methods
            /// <summary>
            /// The color to use when disposing of this GUIColorHelper.
            /// </summary>
            private Color _resetColor;
        #endregion methods

        #region constructors
            /// <summary>
            /// Helper method for executing a delegate with a particular GUI color.
            /// </summary>
            public static void DoWithColor(Color newColor, Action toDo)
            {
                if (toDo == null)
                {
                    return;
                }

                using (var helper = new GUIColorHelper(newColor))
                {
                    toDo();
                }
            }

            /// <summary>
            /// Switches the GUI Color to a new color, and resets it when disposed.
            /// </summary>
            /// <param name="newColor">The new GUI.Color.</param>
            public GUIColorHelper(Color newColor)
            {
                this._resetColor = GUI.color;

                GUI.color = newColor;
            }
        #endregion constructors

        #region methods
            public void Dispose()
            {
                GUI.color = this._resetColor;
            }
        #endregion methods
    }
}