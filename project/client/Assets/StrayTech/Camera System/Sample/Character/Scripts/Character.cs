using UnityEngine;
using StrayTech;

/// Modified version of Unity's Standard Assets ThirdPersonCharacter class
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    #region inspector members
        [SerializeField]
        private float _movingTurnSpeed = 360;
        [SerializeField]
        private float _stationaryTurnSpeed = 180;
        [SerializeField]
        private float _jumpPower = 12f;
        [Range(1f, 4f)]
        [SerializeField]
        private float _gravityMultiplier = 2f;
        [SerializeField]
        private float _runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField]
        private float _moveSpeedMultiplier = 1f;
        [SerializeField]
        private float _animSpeedMultiplier = 1f;
        [SerializeField]
        private float _groundCheckDistance = 0.1f;
    #endregion inspector members

    #region members
        private Rigidbody _rigidbody;
        private Animator _animator;
        private bool _isGrounded;
        private float _origGroundCheckDistance;
        private Vector3 _groundNormal;
        private float _turnAmount;
        private float _forwardAmount;
        private CapsuleCollider _capsule;
        private float _capsuleHeight;
        private Vector3 _capsuleCenter;
        private bool _crouching;
    #endregion members

    #region constructors
        void Start()
        {
            this._animator = GetComponent<Animator>();
            this._rigidbody = GetComponent<Rigidbody>();
            this._capsule = GetComponent<CapsuleCollider>();
            this._capsuleHeight = this._capsule.height;
            this._capsuleCenter = this._capsule.center;

            this._rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            this._origGroundCheckDistance = this._groundCheckDistance;
        }
    #endregion construcors

    #region methods
        public void MoveThirdPerson(Vector3 move, bool crouch, bool jump)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, this._groundNormal);
            this._turnAmount = Mathf.Atan2(move.x, move.z);
            this._forwardAmount = move.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (this._isGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            ScaleCapsuleForCrouching(crouch);
            PreventStandingInLowHeadroom();

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }

        public void MoveFirstPerson(Vector3 move, bool crouch, bool jump)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            //move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, this._groundNormal);
            this._turnAmount = 0.0f;
            this._forwardAmount = move.z;

            // control and velocity handling is different when grounded and airborne:
            if (this._isGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            ScaleCapsuleForCrouching(crouch);
            PreventStandingInLowHeadroom();

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }

        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (this._isGrounded && crouch)
            {
                if (this._crouching) return;
                this._capsule.height = this._capsule.height * 0.5f;
                this._capsule.center = this._capsule.center * 0.5f;
                this._crouching = true;
            }
            else
            {
                Ray crouchRay = new Ray(this._rigidbody.position + Vector3.up * this._capsule.radius * 0.5f, Vector3.up);
                float crouchRayLength = this._capsuleHeight - this._capsule.radius * 0.5f;
                if (Physics.SphereCast(crouchRay, this._capsule.radius * 0.5f, crouchRayLength))
                {
                    this._crouching = true;
                    return;
                }
                this._capsule.height = this._capsuleHeight;
                this._capsule.center = this._capsuleCenter;
                this._crouching = false;
            }
        }

        void PreventStandingInLowHeadroom()
        {
            // prevent standing up in crouch-only zones
            if (!this._crouching)
            {
                Ray crouchRay = new Ray(this._rigidbody.position + Vector3.up * this._capsule.radius * 0.5f, Vector3.up);
                float crouchRayLength = this._capsuleHeight - this._capsule.radius * 0.5f;
                if (Physics.SphereCast(crouchRay, this._capsule.radius * 0.5f, crouchRayLength))
                {
                    this._crouching = true;
                }
            }
        }

        void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            this._animator.SetFloat("Forward", this._forwardAmount, 0.1f, Time.deltaTime);
            this._animator.SetFloat("Turn", this._turnAmount, 0.1f, Time.deltaTime);
            this._animator.SetBool("Crouch", this._crouching);
            this._animator.SetBool("OnGround", this._isGrounded);
            if (!this._isGrounded)
            {
                this._animator.SetFloat("Jump", this._rigidbody.velocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(this._animator.GetCurrentAnimatorStateInfo(0).normalizedTime + this._runCycleLegOffset, 1);
            float jumpLeg = (runCycle < 0.5f ? 1 : -1) * this._forwardAmount;
            if (this._isGrounded)
            {
                this._animator.SetFloat("JumpLeg", jumpLeg);
            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (this._isGrounded && move.magnitude > 0)
            {
                this._animator.speed = this._animSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                this._animator.speed = 1;
            }
        }

        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * this._gravityMultiplier) - Physics.gravity;
            this._rigidbody.AddForce(extraGravityForce);

            this._groundCheckDistance = this._rigidbody.velocity.y < 0 ? this._origGroundCheckDistance : 0.01f;
        }


        void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && !crouch && this._animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {
                // jump!
                this._rigidbody.velocity = new Vector3(this._rigidbody.velocity.x, this._jumpPower, this._rigidbody.velocity.z);
                this._isGrounded = false;
                this._animator.applyRootMotion = false;
                this._groundCheckDistance = 0.1f;
            }
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(this._stationaryTurnSpeed, this._movingTurnSpeed, this._forwardAmount);
            transform.Rotate(0, this._turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * this._groundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, this._groundCheckDistance))
            {
                this._groundNormal = hitInfo.normal;
                this._isGrounded = true;
                this._animator.applyRootMotion = true;
            }
            else
            {
                this._isGrounded = false;
                this._groundNormal = Vector3.up;
                this._animator.applyRootMotion = false;
            }
        }
    #endregion methods

    #region monobehaviour callbacks
        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (this._isGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (this._animator.deltaPosition * this._moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = this._rigidbody.velocity.y;
                this._rigidbody.velocity = v;
            }
        }
    #endregion monobehaviour callbacks
}
