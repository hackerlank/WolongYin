using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif

namespace StrayTech
{
    [ExecuteInEditMode]
    public abstract class CameraSystemTriggerBase : MonoBehaviour
    {
        #region inner types
            public enum TriggerColliderType
            {
                Box = 0,
                Sphere = 1,
                ConvexMesh = 2
            }
        #endregion inner types

        #region inspector members
            [SerializeField]
            [Tooltip("The type of collider to use.")]
            private TriggerColliderType _triggerColliderType = TriggerColliderType.Box;

            [SerializeField]
            [Tooltip("Mesh for Convex Mesh collider trigger.")]
            private Mesh _mesh;

            [SerializeField]
            [Tooltip("The color of the volume in the editor.")]
            private Color _volumeColor = new Color(0.0f, 1.0f, 0.0f, 0.25f);

            [SerializeField]
            [Tooltip("Should this volume render as a solid in editor?")]
            private bool _renderSolidVolume = true;

            [SerializeField]
            [Tooltip("Render volume only when selected?")]
            private bool _renderOnlyWhenSelected = false;

            [SerializeField]
            [Tooltip("Filter collision by tag? (Blank means no tag filter)")]
            protected string _tagFilter;

            [SerializeField]
            [Tooltip("The layers that will trigger the volume.")]
            protected LayerMask _layerMask = -1;

            [SerializeField]
            [Tooltip("Use once then disable.")]
            protected bool _singleUseTrigger = false;
        #endregion inspector members

        #region members
            private bool _triggerEnabled = true;
            private Collider _collider = null;

            private List<ITriggerGate> _triggerGates;

            private bool _didTrigger = false;
        #endregion

        #region constructors
            private void Start()
            {
                if (Application.isPlaying == true)
                {
                    if (CameraSystem.Instance == null)
                    {
                        Debug.LogErrorFormat(this, "{0} could not find an instance of CameraSystem!", this);
                        this.enabled = false;
                        return;
                    }

                    switch (this._triggerColliderType)
                    {
                        case TriggerColliderType.Box:
                            this._collider = this.gameObject.AddOrGetComponent<BoxCollider>();
                            break;
                        case TriggerColliderType.Sphere:
                            this._collider = this.gameObject.AddOrGetComponent<SphereCollider>();
                            break;
                        case TriggerColliderType.ConvexMesh:
                            this._collider = this.gameObject.AddOrGetComponent<MeshCollider>();
                            (this._collider as MeshCollider).sharedMesh = this._mesh;
                            (this._collider as MeshCollider).convex = true;
                            break;
                    }
                    
                    this._collider.isTrigger = true;
                    this._collider.hideFlags = HideFlags.HideInInspector;
                    
                    this._triggerGates = new List<ITriggerGate>(this.gameObject.GetInterfacesInChildren<ITriggerGate>(true, true));
                }
            }
        #endregion construcors

        #region methods
            protected abstract void TriggerEntered();
            protected abstract void TriggerExited();
            
            private void OnTriggerEnter(Collider other)
            {
                if (Application.isPlaying == false || this.enabled == false || this._triggerEnabled == false)
                {
                    return;
                }

                if (other == null || other.gameObject == null)
                {
                    return;
                }

                // filter collision with provided layer mask
                if ((this._layerMask.value & (1 << other.gameObject.layer)) != 0)
                {
                    // filter collision by tag if provided
                    if (string.IsNullOrEmpty(this._tagFilter) != true)
                    {
                        if (other.gameObject.tag != this._tagFilter)
                        {
                            return;
                        }
                    }

                    if (IsTriggerGated() == true)
                    {
                        for (int i = 0; i < this._triggerGates.Count; i++)
                        {
                            this._triggerGates[i].TriggerWasEntered(other);
                        }

                        StartCoroutine("SpinOnGatedEnter");
                    }
                    else
                    {
                        this._didTrigger = true;
                        TriggerEntered();
                    }
                }

            }

            private void OnTriggerExit(Collider other)
            {
                if (Application.isPlaying == false || this.enabled == false || this._triggerEnabled == false)
                {
                    return;
                }

                if (other == null || other.gameObject == null)
                {
                    return;
                }

                // filter collision with provided layer mask
                if ((this._layerMask.value & (1 << other.gameObject.layer)) != 0)
                {
                    // filter collision by tag if provided
                    if (string.IsNullOrEmpty(this._tagFilter) != true)
                    {
                        if (other.gameObject.tag != this._tagFilter)
                        {
                            return;
                        }
                    }

                    StopCoroutine("SpinOnGatedEnter");

                    if (this._didTrigger == true)
                    {
                        TriggerExited();
                        this._didTrigger = false;
                    }

                    if (this._singleUseTrigger == true)
                    {
                        this._triggerEnabled = false;
                    }
                }
            }

            private IEnumerator SpinOnGatedEnter()
            {
                while (true)
                {
                    while (IsTriggerBlocked() == true || CameraSystem.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning)
                    {
                        yield return null;
                    }

                    this._didTrigger = true;
                    TriggerEntered();

                    while (IsTriggerBlocked() == false || CameraSystem.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning)
                    {
                        yield return null;
                    }

                    TriggerExited();
                    this._didTrigger = false;

                    if (this._singleUseTrigger == true)
                    {
                        this._triggerEnabled = false;
                        break;
                    }
                }
            }

            private bool IsTriggerGated()
            {
                return (this._triggerGates.Count > 0);
            }

            private bool IsTriggerBlocked()
            {
                if (this._triggerGates.Count == 0)
                { 
                    return true; 
                }

                bool blocked = false;

                for (int i = 0; i < this._triggerGates.Count; i++)
                {
                    if (this._triggerGates[i].IsActive == true && this._triggerGates[i].IsTriggerBlocked() == true)
                    {
                        blocked = true;
                        break;
                    }
                }

                return blocked;
            }
        #endregion methods

        #region monobehaviour callbacks
#if UNITY_EDITOR
            private void OnDrawGizmos()
            {
                if (this._renderOnlyWhenSelected == true)
                {
                    if (UnityEditor.Selection.activeTransform == null || UnityEditor.Selection.activeTransform.GetComponentsInChildren<Transform>(true).Contains(this.transform) == false)
                    {
                        return;
                    }
                }

                Matrix4x4 gizmoMatrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
                Gizmos.matrix = gizmoMatrix;
                Gizmos.color = this._volumeColor;

                if (this._renderSolidVolume == true)
                {
                    switch (this._triggerColliderType)
                    {
                        case TriggerColliderType.Box:
                            Gizmos.DrawCube(Vector3.zero, this.transform.lossyScale);
                            break;
                        case TriggerColliderType.Sphere:
                            Gizmos.DrawSphere(Vector3.zero, this.transform.lossyScale.x * 0.5f);
                            break;
                        case TriggerColliderType.ConvexMesh:
                            Gizmos.DrawMesh(this._mesh, Vector3.zero, Quaternion.identity, this.transform.lossyScale);
                            break;
                    }
                }

                if (UnityEditor.Selection.activeTransform != null && UnityEditor.Selection.activeTransform.GetComponentsInChildren<Transform>(true).Contains(this.transform) == true)
                {
                    Gizmos.color = Color.white;
                }

                switch (this._triggerColliderType)
                {
                    case TriggerColliderType.Box:
                        Gizmos.DrawWireCube(Vector3.zero, this.transform.lossyScale);
                        break;
                    case TriggerColliderType.Sphere:
                        Gizmos.DrawWireSphere(Vector3.zero, this.transform.lossyScale.x * 0.5f);
                        break;
                    case TriggerColliderType.ConvexMesh:
                        Gizmos.DrawWireMesh(this._mesh, Vector3.zero, Quaternion.identity, this.transform.lossyScale);
                        break;
                }
            }
#endif

            private void OnDestroy()
            {
                if (Application.isPlaying == true)
                {
                    Component.Destroy(this._collider);
                }
            }
        #endregion monobehaviour callbacks
    }
}