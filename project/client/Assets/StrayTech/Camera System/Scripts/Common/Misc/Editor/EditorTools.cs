using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace StrayTech
{
    /// <summary>
    /// Tools for the editor
    /// </summary>
    public static class EditorTools
    {
        /// <summary>
        /// Returns a blank usable 1x1 white texture.
        /// </summary>

        static public Texture2D blankTexture
        {
            get
            {
                return EditorGUIUtility.whiteTexture;
            }
        }

        /// <summary>
        /// Draw a visible separator in addition to adding some padding.
        /// </summary>

        static public void DrawSeparator()
        {
            GUILayout.Space(12f);

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = blankTexture;
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.color = new Color(0f, 0f, 0f, 0.25f);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        static public void DrawDivider(float padding)
        {
            GUILayout.Space(padding);

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = blankTexture;
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.color = new Color(1f, 1f, 1f, 0.25f);
                GUI.DrawTexture(new Rect(0f, rect.yMin + (padding * 0.5f), Screen.width, 1f), tex);
                GUI.color = Color.white;
            }
        }
    }
}
