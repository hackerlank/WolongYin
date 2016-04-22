using UnityEngine;
using System.Collections;

namespace StrayTech
{
    public class UserDefinedFlagTriggerGate : MonoBehaviour, ITriggerGate
    {
        #region inspector members
            [SerializeField]
            [Tooltip("If this user defined flag is false, OnTriggerEnter logic will be bypassed.")]
            private string _userDefinedFlagName;
        #endregion inspector members

        #region properties
            public bool IsActive { get { return this.gameObject.activeInHierarchy; } }
        #endregion properties

        #region constructors
            private void Start()
            {
                if(CameraSystem.Instance == null)
                {
                    Debug.LogErrorFormat(this, "{0} could not find an instance of CameraSystem!", this);
                    this.enabled = false;
                    return;
                }
            }
        #endregion construcors

        #region methods
            public void TriggerWasEntered(Collider other) { }

            public bool IsTriggerBlocked()
            {
                return (CameraSystem.Instance.GetUserDefinedFlagValue(this._userDefinedFlagName) == false);
            }
        #endregion methods
    }
}