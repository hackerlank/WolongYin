using System;
using UnityEngine;

namespace StrayTech
{
    [ExecuteInEditMode]
    public class Grayscale : MonoBehaviour
    {
        #region inspector members
            [SerializeField]
            private Shader _shader;
        #endregion inspector members

        #region members
            private Material _material;
        #endregion members

        #region constructors
            private void Start()
            {
                // Disable if we don't support image effects
                if (!SystemInfo.supportsImageEffects)
                {
                    this.enabled = false;
                    return;
                }

                // Disable the image effect if the shader can't
                // run on the users graphics card
                if (!this._shader || !this._shader.isSupported)
                    this.enabled = false;
            }
        #endregion construcors

        #region monobehaviour callbacks
            private void OnRenderImage(RenderTexture source, RenderTexture destination)
            {
                if (this._material == null)
                {
                    this._material = new Material(this._shader);
                    this._material.hideFlags = HideFlags.HideAndDontSave;
                }

                Graphics.Blit(source, destination, this._material);
            }

            private void OnDisable()
            {
                if (this._material)
                {
                    DestroyImmediate(this._material);
                }
            }
        #endregion monobehaviour callbacks
    }
}
