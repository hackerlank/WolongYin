using UnityEngine;

namespace StrayTech
{
    /// <summary>
    /// Forces the attached GameObject to have a unit (1,1,1) scale.
    /// <para>Additionally if the GameObject has a Collider on it, scales it so it's size is the same in the world after it's parent's scale is set to (1,1,1).</para>
    /// </summary>
    [ExecuteInEditMode]
    public class EnforceUnitScale : MonoBehaviour
    {
        #region constructors
            private void Awake()
            {
                if (Application.isPlaying == true)
                {
                    Component.Destroy(this);
                }
            }

            private void Start()
            {
                this.UpdateCollider();
            }

            /// <summary>
            /// Rescale the attached box/sphere collider so that when the scale of this GameObject is set to (1,1,1), the collider is functionally the same size.
            /// </summary>
            private void UpdateCollider()
            {
                if (this.transform.localScale == Vector3.one)
                {
                    return;
                }

                var colliders = this.gameObject.GetComponents<Collider>();

                foreach (var collider in colliders)
                {
                    var colliderType = collider.GetType();

                    if (colliderType == typeof(BoxCollider))
                    {
                        this.UpdateBoxCollider(collider as BoxCollider);
                    }
                    else if (colliderType == typeof(SphereCollider))
                    {
                        this.UpdateSphereCollider(collider as SphereCollider);
                    }
                }
            }

            private void UpdateSphereCollider(SphereCollider sphereCollider)
            {
                var biggestComponent = Mathf.Max(Mathf.Max(this.transform.localScale.x, this.transform.localScale.y), this.transform.localScale.z);

                sphereCollider.radius *= biggestComponent;
            }

            private void UpdateBoxCollider(BoxCollider boxCollider)
            {
                boxCollider.size = Vector3.Scale(boxCollider.size, this.transform.localScale);
            }
        #endregion constructors

        #region monobehaviour callbacks
            private void Update()
            {
                this.transform.localScale = Vector3.one;
            }
        #endregion monobehaviour callbacks
    }
}