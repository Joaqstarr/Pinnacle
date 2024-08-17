﻿using UnityEngine;

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
            if (Controls.leftArmPressed || Controls.rightArmPressed)
            {
                if (Player.CheckForClimbable() != null)
                {
                    Player.ChangeToClimbingState();  
                    return;
                }
                
            }
            _isGrounded = GroundCheck();
            
            Player.CameraMovement();
            
        }

        public override void OnFixedUpdateState()
        {
            Move();
            Gravity();
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

        private bool GroundCheck()
        {
             Collider[] result = Physics.OverlapSphere(Player.transform.position + Data.groundCheckPos, 0.1f, Data.groundLayers);
             if (result.Length == null) return false;

             if (result.Length == 0) return false;
             return true;

        }
        private void Jump()
        {
            if(!_isGrounded) return;
            Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

            Rb.AddForce(Player.transform.up * Data.jumpStrength, ForceMode.Impulse);
        }



    }
}