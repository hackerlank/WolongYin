using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public class CameraCollision : MonoBehaviourSingleton<CameraCollision>
    {
        #region inner types
            public enum CollisionTestType
            {
                SphereCast,
                RayCast
            }
        #endregion inner types

        #region inspector members
            [SerializeField]
            [Tooltip("Globally toggle camera collision on and off.")]
            private bool _useCameraCollision = false;

            [SerializeField]
            [Tooltip("Type of camera collsion.")]
            private CollisionTestType _testType = CollisionTestType.RayCast;

            [SerializeField]
            [Tooltip("Radius of the sphere for sphere casts.")]
            private float _sphereRadius = 0.5f;

            [SerializeField]
            [Tooltip("Layers to collide with.")]
            private LayerMask _collisionLayerMask;
        #endregion inspector members

        #region properties
            public bool UseCameraCollision { get { return this._useCameraCollision; } }
            public CollisionTestType TestType { get { return this._testType; } }
            public float SphereRadius { get { return _sphereRadius; } }
        #endregion properties

        #region methods
            public void PreventCameraCollision(Camera camera)
            {
                if(this._useCameraCollision == false)
                {
                    return;
                }

                if (CameraSystem.Instance.CameraTarget == null)
                {
                    return;
                }

                Vector3 cameraOriginalPosition = camera.transform.position;
                Vector3 targetPosition = CameraSystem.Instance.CameraTarget.position;
                Vector3 fromTargetToCamera = (cameraOriginalPosition - targetPosition).normalized;
                float distanceToTarget = Vector3.Distance(cameraOriginalPosition, targetPosition);

                RaycastHit hitInfo;
                switch (this._testType)
                {
                    case CollisionTestType.SphereCast:
                        if(Physics.SphereCast(targetPosition, this._sphereRadius, fromTargetToCamera, out hitInfo, distanceToTarget, this._collisionLayerMask))
                        {
                            camera.transform.position = targetPosition + (fromTargetToCamera * hitInfo.distance);
                        }
                        break;
                    case CollisionTestType.RayCast:
                         if(Physics.Raycast(targetPosition, fromTargetToCamera, out hitInfo, distanceToTarget, this._collisionLayerMask))
                         {
                             camera.transform.position = targetPosition + (fromTargetToCamera * (hitInfo.distance - 0.1f));
                         }
                        break;
                }
            }
        #endregion methods
    }
}