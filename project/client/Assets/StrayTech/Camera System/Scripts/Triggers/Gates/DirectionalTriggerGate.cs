using UnityEngine;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StrayTech
{
    public class DirectionalTriggerGate : MonoBehaviour, ITriggerGate
    {
        #region inspector members
            [SerializeField]
            [Range(0.0f, 360.0f)]
            [Tooltip("The primary direction the volume can be triggered from.")]
            private float _angle = 0.0f;

            [SerializeField]
            [Range(0.0f, 180.0f)]
            [Tooltip("The span of the primary direction the volume can be triggered from.")]
            private float _angleSpan = 90.0f;
        #endregion inspector members

        #region members
            private bool _enteredFromValidDirection = false;
            private Vector3 _validDirection;
        #endregion members

        #region properties
            public bool IsActive { get { return this.gameObject.activeInHierarchy; } }
        #endregion properties

        #region constructors
            private void Start()
            {
                Vector3 transformForward = this.transform.forward;
                transformForward.y = 0;
                this._validDirection = Quaternion.AngleAxis(this._angle, Vector3.up) * transformForward.normalized;

                if (CameraSystem.Instance == null)
                {
                    Debug.LogErrorFormat(this, "{0} could not find an instance of CameraSystem!", this);
                    this.enabled = false;
                    return;
                }
            }
        #endregion construcors

        #region methods
            public void TriggerWasEntered(Collider other)
            {
                Vector3 enteredVector = other.transform.position - this.transform.position;
                enteredVector.y = 0;

                Vector3 enteredDirection = enteredVector.normalized;


                float angleDifference = Vector3.Angle(enteredDirection, this._validDirection);

                if (angleDifference <= this._angleSpan)
                {
                    this._enteredFromValidDirection = true;
                }
                else
                {
                    this._enteredFromValidDirection = false;
                }
            }

            public bool IsTriggerBlocked()
            {
                return (this._enteredFromValidDirection == false);
            }
        #endregion methods

        #region monobehaviour callbacks
#if UNITY_EDITOR
            private void OnDrawGizmos()
            {
                if (UnityEditor.Selection.activeTransform == null || UnityEditor.Selection.activeTransform.GetComponentsInChildren<Transform>(true).Contains(this.transform) == false)
                {
                    return;
                }
                Vector3 transformForward = this.transform.forward;
                transformForward.y = 0;
                Vector3 validDirection = Quaternion.AngleAxis(this._angle - (this._angleSpan), Vector3.up) * transformForward.normalized;

                Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.1f);
                Handles.DrawSolidArc(this.transform.position, Vector3.up, validDirection, this._angleSpan * 2.0f, 2.0f);
            }
#endif
        #endregion monobehaviour callbacks
    }
}