using System;
using UnityEngine;
using StrayTech;

/// Modified version of Unity's Standard Assets ThirdPersonUserControl class
[RequireComponent(typeof(Character))]
public class CharacterControl : MonoBehaviour
{
    #region members
        private Character _character = null;
        private Vector3 _camForward = Vector3.zero;
        private Vector3 _move = Vector3.zero;
        private bool _jump = false;
    #endregion members

    #region constructors
        private void Start()
        {
            this._character = GetComponent<Character>();
        }
    #endregion construcors

    #region monobehaviour callbacks
        private void Update()
        {
            if (!this._jump)
            {
                this._jump = Input.GetButtonDown("Jump");
            }
        }

        private void FixedUpdate()
        {
            if (CameraSystem.Instance == null || CameraSystem.Instance.CurrentCamera == null || CameraSystem.Instance.CurrentCameraStateDefinition == null)
            {
                return;
            }

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            Quaternion cameraRotation;
            CameraSystem.CameraStateEnum cameraStateType;

            if (CameraSystem.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning)
            {
                cameraRotation = CameraSystem.Instance.NextCamera.transform.rotation;
                cameraStateType = CameraSystem.Instance.NextCameraStateDefinition.State.StateType;
            }
            else
            {
                cameraRotation = CameraSystem.Instance.CurrentCamera.transform.rotation;
                cameraStateType = CameraSystem.Instance.CurrentCameraStateDefinition.State.StateType;
            }

            if (cameraStateType == CameraSystem.CameraStateEnum.FirstPerson)
            {
                this._move = (vertical * Vector3.forward) * 0.5f;
                if (Input.GetKey(KeyCode.LeftShift)) this._move *= 2.0f;
                this._character.MoveFirstPerson(this._move, crouch, this._jump);
            }
            else
            {
                this._camForward = Vector3.Scale(cameraRotation * Vector3.forward, new Vector3(1, 0, 1)).normalized;
                this._move = (vertical * this._camForward + horizontal * (cameraRotation * Vector3.right)) * 0.5f;
                if (Input.GetKey(KeyCode.LeftShift)) this._move *= 2.0f;
                this._character.MoveThirdPerson(this._move, crouch, this._jump);
            }

            
           
            this._jump = false;

            if (crouch)
            {
                CameraSystem.Instance.SetUserDefinedFlagValue("PlayerInput_Crouch", true);
            }
            else
            {
                CameraSystem.Instance.SetUserDefinedFlagValue("PlayerInput_Crouch", false);
            }


            if (Input.GetKey(KeyCode.E))
            {
                CameraSystem.Instance.SetUserDefinedFlagValue("PlayerInput_Use", true);
            }
            else
            {
                CameraSystem.Instance.SetUserDefinedFlagValue("PlayerInput_Use", false);
            }
        }
    #endregion monobehaviour callbacks
}
