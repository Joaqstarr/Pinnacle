using Unity.VisualScripting;
using UnityEngine;

namespace Player.States
{
    public class ClimbingState : BaseState
    {
        private Arm _leftArm;
        private Arm _rightArm;
        public ClimbingState(PlayerBrain player, PlayerControls controls, PlayerData data, Rigidbody rb, Arm leftArm, Arm rightArm) : base(player, controls, data, rb)
        {
            _leftArm = leftArm;
            _rightArm = rightArm;
        }

        public override void OnEnterState()
        {
            CheckArmHolds();

        }

        public override void OnExitState()
        {
            
        }

        public override void OnUpdateState()
        {
            Player.CameraMovement();
            CheckArmHolds();
            
        }

        public override void OnFixedUpdateState()
        {
            Movement();
        }


        private void Movement()
        {
            Vector3 forceToApply = (Player.transform.up * (Controls.moveInput.y * Data.climbMoveSpeed.y)) +
                                   (Player.transform.right * (Controls.moveInput.x * Data.climbMoveSpeed.x));
            
            Rb.AddForce(forceToApply, ForceMode.Force);
        }
        private void ConnectArm(Arm arm, Climbable hold)
        {
            arm.AttachToHold(hold);
        }

        private void DisconnectArm(Arm arm)
        {
            arm.LetGo();
        }

        private void DisconnectLeftArm()
        {
            DisconnectArm(_leftArm);
        }

        private void DisconnectRightArm()
        {
            DisconnectArm(_rightArm);
        }
        private void ConnectLeftArm(Climbable hold)
        {
            ConnectArm(_leftArm, hold);
        }
        private void ConnectRightArm(Climbable hold)
        {
            ConnectArm(_rightArm, hold);
        }

        private void CheckArmHolds()
        {
            Climbable hold = Player.CheckForClimbable();
            
            CheckArm(_leftArm, Controls.leftArmPressed, hold);
            
            CheckArm(_rightArm, Controls.rightArmPressed, hold);

        }

        private void CheckArm(Arm arm, bool input, Climbable hold)
        {
            if (input)
            {
                if(hold != null)
                    if(!arm.isAttached)
                        ConnectArm(arm, hold);
            }
            else
            {
                if(arm.isAttached)
                    DisconnectArm(arm);
            }
        }
    }
}