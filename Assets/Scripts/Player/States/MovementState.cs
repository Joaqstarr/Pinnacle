using UnityEngine;

namespace Player.States
{
    public class MovementState : BaseState
    {
        private bool _isGrounded = false;
        private float _bonusGrav = 0;
        public MovementState(PlayerBrain player, PlayerControls controls, PlayerData data,Rigidbody rb) : base(player, controls, data, rb)
        {
        }

        public override void OnEnterState()
        {
            Controls.JumpPressed += Jump;
            
        }

        public override void OnExitState()
        {
            Controls.JumpPressed -= Jump;

        }

        public override void OnUpdateState()
        {
            if (CheckHandHolds()) return;
            
            _isGrounded = Player.GroundCheck();
            
            Player.CameraMovement();
            
        }

        private bool CheckHandHolds()
        {
            if(!Player.isBuildModeEnabled)
                if (Controls.leftArmPressed || Controls.rightArmPressed)
                {
                    if (Player.CheckForClimbable() != null)
                    {
                        Player.ChangeToClimbingState();
                        return true;
                    }
                    
                }

            return false;
        }

        public override void OnFixedUpdateState()
        {
            Move();
            Gravity();
        }

        public override void OnEnterBuildMode()
        {
        }

        public override void OnExitBuildMode()
        {
            
        }

        private void Gravity()
        {
            if (_isGrounded)
            {
                _bonusGrav = 0;
                return;
            }

            _bonusGrav = Mathf.Lerp(_bonusGrav, Data.gravity, Time.deltaTime * Data.bonusGravitySpeed);
            Rb.AddForce(Vector3.up * _bonusGrav, ForceMode.Force);
        }

        private void Move()
        {
            Vector3 currrentVelocity = Rb.velocity;
            Vector3 targetVelocity = new Vector3(Controls.moveInput.x, 0, Controls.moveInput.y);
            targetVelocity *= Data.moveSpeed * 10;

            targetVelocity = Player.transform.TransformDirection(targetVelocity);

            Vector3 velocityChange = targetVelocity - currrentVelocity;
            velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

            Vector3.ClampMagnitude(velocityChange, Data.maxForce);


            ForceMode mode = ForceMode.VelocityChange;
            if (!_isGrounded)
            {
                velocityChange = targetVelocity;
                velocityChange *= Data.airSpeedModifier;
            }

            velocityChange *= Time.deltaTime;

            Rb.AddForce(velocityChange, mode);
           
            
        }


        private void Jump()
        {
            if(Player.isBuildModeEnabled) return;
            if(!_isGrounded) return;
            Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

            Rb.AddForce(Player.transform.up * Data.jumpStrength, ForceMode.Impulse);
        }



    }
}