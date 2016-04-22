using UnityEngine;
using System.Collections;
using StrayTech;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
    #region inspector members
        [SerializeField]
        private Text _currentControlsText;
        [SerializeField]
        private Text _cameraStateText;
        [SerializeField]
        private Text _cameraModifierText;
    #endregion inspector members

    #region monobehaviour callbacks
        private void Update()
        {
            if (CameraSystem.Instance != null)
            {
                if (this._currentControlsText != null)
                {
                    bool crouch = Input.GetKey(KeyCode.C);
                    bool use = Input.GetKey(KeyCode.E);
                    bool run = Input.GetKey(KeyCode.LeftShift);
                    bool jump = Input.GetButtonDown("Jump");

                    float horizontal = Input.GetAxis("Horizontal");
                    float vertical = Input.GetAxis("Vertical");

                    this._currentControlsText.text = "";
                    if (string.IsNullOrEmpty(this._currentControlsText.text) == true)
                    {
                        if(Mathf.Approximately(vertical, 0.0f) == false)
                        {
                            this._currentControlsText.text += (vertical > 0) ? "▲, " : "▼, ";
                        }

                        if(Mathf.Approximately(horizontal, 0.0f) == false)
                        {
                            this._currentControlsText.text += (horizontal > 0) ? "►, " : "◄, ";
                        }

                        if (jump == true)
                        {
                            this._currentControlsText.text += "Jump, ";
                        }

                        if (run == true)
                        {
                            this._currentControlsText.text += "Run, ";
                        }

                        if (crouch == true)
                        {
                            this._currentControlsText.text += "Crouch, ";
                        }

                        if (use == true)
                        {
                            this._currentControlsText.text += "Use, ";
                        }
                    }
                }

                if (this._cameraStateText != null)
                {
                    if (CameraSystem.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning)
                    {
                        this._cameraStateText.text = CameraSystem.Instance.NextCameraStateDefinition.State.StateType.ToString();
                    }
                    else if (CameraSystem.Instance.CurrentCameraStateDefinition != null)
                    {
                        this._cameraStateText.text = CameraSystem.Instance.CurrentCameraStateDefinition.State.StateType.ToString();
                    }
                    else
                    {
                        this._cameraStateText.text = "";
                    }
                }

                if (this._cameraModifierText != null)
                {
                    this._cameraModifierText.text = "";
                    foreach(CameraStateModifierBase modifier in CameraSystem.Instance.CameraStateModifiers)
                    {
                        if (string.IsNullOrEmpty(this._cameraModifierText.text) == true)
                        {
                            this._cameraModifierText.text = modifier.Name + "\n";
                        }
                        else
                        {
                            this._cameraModifierText.text += modifier.Name + "\n";
                        }
                    }
                }
            }
        }
    #endregion monobehaviour callbacks
}
