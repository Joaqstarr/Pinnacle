using Unity.VisualScripting;

namespace Player.States
{
    public class ClimbingState : BaseState
    {
        private Arm _leftArm;
        private Arm _rightArm;
        public ClimbingState(PlayerBrain player, PlayerControls controls, PlayerData data, Arm leftArm, Arm rightArm) : base(player, controls, data)
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