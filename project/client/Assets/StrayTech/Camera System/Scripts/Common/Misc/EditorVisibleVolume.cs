using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StrayTech
{
    /// <summary>
    /// Represents a volume in worldspace that is displayed in the editor.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class EditorVisibleVolume : MonoBehaviour
    {
        #region inspector members
            /// <summary>
            /// The color of the volume in the editor.
            /// </summary>
            [SerializeField]
            private Color _volumeColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            /// <summary>
            /// Should this render gizmos? 
            /// </summary>
            [SerializeField]
            private bool _shouldRender = true;
            /// <summary>
            /// Render gizmos only when selected? 
            /// </summary>
            [SerializeField]
            private bool _shouldRenderOnlyWhenSelected = false;
        #endregion inspector members

        #region members
            private BoxCollider _collider;
        #endregion members

        #region properties
            /// <summary>
            /// The color of the volume in the editor.
            /// </summary>
            public Color VolumeColor
            {
                get { return _volumeColor; }
                set { _volumeColor = value; }
            }
        #endregion properties

        #region monobehaviour callbacks
#if UNITY_EDITOR
            private void OnDrawGizmos()
            {
                if (_shouldRender == false)
                    return;

                if (this._collider == null)
                {
                    this._collider = this.gameObject.transform.GetComponent<BoxCollider>();
                }

                if (this._collider == null)
                {
                    return;
                }

                if (this._shouldRenderOnlyWhenSelected == true)
                {
                    if (UnityEditor.Selection.activeTransform == null)
                    {
                        return;
                    }

                    if (UnityEditor.Selection.activeTransform.GetComponentsInChildren<Transform>(true).Contains(this.transform) == false)
                    {
                        return;
                    }
                }

                Matrix4x4 gizmoMatrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.lossyScale);
                Gizmos.matrix = gizmoMatrix;
                Gizmos.color = this._volumeColor;

                Gizmos.DrawCube(this._collider.center, this._collider.size);
            }
#endif
        #endregion monobehaviour callbacks
    }
}