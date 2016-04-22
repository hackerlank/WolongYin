using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public class SmoothDampFollowTarget : MonoBehaviour
    {
        #region inspector members
            /// <summary>
            /// The transform to follow
            /// </summary>
            [SerializeField]
            private Transform _followTarget;

            /// <summary>
            /// An offset from the target to interpolate to.
            /// </summary>
            [SerializeField]
            private Vector3 _targetOffset;

            [SerializeField]
            private float _smoothTime = 0.25f;

            [SerializeField]
            private bool _useFixedUpdate = false;
        #endregion inspector members

        #region members
            private Vector3[] _vector3s = new Vector3[4];
        #endregion members

        #region constructors
            private void Start()
            {
                this.transform.position = this._followTarget.position + (this._followTarget.rotation * this._targetOffset);
            }
        #endregion construcors

        #region monobehaviour callbacks
            private void Update()
            {
                if (this._useFixedUpdate == false)
                {
                    DoUpdate(Time.deltaTime);
                }
            }

            private void FixedUpdate()
            {
                if (this._useFixedUpdate == true)
                {
                    DoUpdate(Time.fixedDeltaTime);
                }
            }

            private void DoUpdate(float deltaTime)
            {
                // from values
                this._vector3s[0] = this._vector3s[1] = this.transform.position;

                //to values:
                this._vector3s[1] = this._followTarget.position + (this._followTarget.rotation * this._targetOffset);

                //calculate:
                this._vector3s[3].x = Mathf.SmoothDamp(this._vector3s[0].x, this._vector3s[1].x, ref this._vector3s[2].x, this._smoothTime * deltaTime);
                this._vector3s[3].y = Mathf.SmoothDamp(this._vector3s[0].y, this._vector3s[1].y, ref this._vector3s[2].y, this._smoothTime * deltaTime);
                this._vector3s[3].z = Mathf.SmoothDamp(this._vector3s[0].z, this._vector3s[1].z, ref this._vector3s[2].z, this._smoothTime * deltaTime);

                this.transform.position = this._vector3s[3];

                this.transform.rotation = this._followTarget.rotation;
            }
        #endregion monobehaviour callbacks
    }
}