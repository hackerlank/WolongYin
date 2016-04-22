using UnityEngine;
using System.Collections;

namespace StrayTech
{
    [RequireComponent(typeof(Camera))]
    public class CrossfadePostProcess : MonoBehaviour
    {
        private Material _material;

        private void Start()
        {
            if (CameraSystem.Instance == null)
            {
                Debug.LogErrorFormat(this, "{0} could not find a CameraMasterControl in the scene!", this);
                this.enabled = false;
                return;
            }

            this._material = new Material(Shader.Find("Hidden/CrossfadeCamera"));
            this._material.hideFlags = HideFlags.HideAndDontSave;
            this._material.SetTexture("_CrossfadeTexture", CameraSystem.Instance.CameraRenderTexture);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CameraSystem.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning &&
                (CameraSystem.Instance.CurrentTransitionType == CameraSystem.StateTransitionTypeInternal.Crossfade ||
                CameraSystem.Instance.CurrentTransitionType == CameraSystem.StateTransitionTypeInternal.InterpolatedCrossfade))
            {
                this._material.SetFloat("_Alpha", CameraSystem.Instance.CurrentInterpolationCurveSample);
                Graphics.Blit(source, destination, this._material);
            }
            else
            {
                Graphics.Blit(source, destination);
                return;
            }
        }

        void OnDestroy()
        {
            if (this._material)
                DestroyImmediate(this._material);
        }
    }
}