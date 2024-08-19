using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.States
{
    public class ClimbingState : BaseState
    {
        private Arm _leftArm;
        private Arm _rightArm;
        private bool _enabledBuild = false;

        public ClimbingState(PlayerBrain player, PlayerControls controls, PlayerData data, Rigidbody rb, Arm leftArm, Arm rightArm) : base(player, controls, data, rb)
        {
            _leftArm = leftArm;
            _rightArm = rightArm;
        }

        public override void OnEnterState()
        {
            Controls.JumpPressed += OnAttemptMantle;
            _enabledBuild = false;
            CheckArmHolds();

        }

        public override void OnExitState()
        {
            Controls.JumpPressed -= OnAttemptMantle;
            DisconnectLeftArm();
            DisconnectRightArm();
            EnterNormalTime();
        }

        public override void OnUpdateState()
        {
            Player.CameraMovement();
            if(!Player.isBuildModeEnabled)
                CheckArmHolds();

            if (Player.GroundCheck() && IsLetgo())
            {
                Player.ChangeToMovementState();
                return;
            }
        }

        public override void OnFixedUpdateState()
        {
            Movement();
             
        }

        public override void OnEnterBuildMode()
        {
            _enabledBuild = true;

        }

        public override void OnExitBuildMode()
        {
            
        }


        private void Movement()
        {
            
            Vector3 forwardDir = (IsLetgo())? Player.transform.forward : Player.transform.up;
            Vector3 forceToApply = (forwardDir* (Controls.moveInput.y * Data.climbMoveSpeed.y)) +
                                   (Player.transform.right * (Controls.moveInput.x * Data.climbMoveSpeed.x));
            
            Rb.AddForce(forceToApply, ForceMode.Force);
        }

        private bool IsLetgo()
        {
            return !_leftArm.isAttached && !_rightArm.isAttached;
        }
        private void ConnectArm(Arm arm, PlayerBrain.ClimbableData holdData)
        {
            arm.AttachToHold(holdData);
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
        private void ConnectLeftArm(PlayerBrain.ClimbableData hold)
        {
            ConnectArm(_leftArm, hold);
        }
        private void ConnectRightArm(PlayerBrain.ClimbableData hold)
        {
            ConnectArm(_rightArm, hold);
        }

        private void CheckArmHolds()
        {
            PlayerBrain.ClimbableData holdData = Player.CheckForClimbable();
            
            CheckArm(_leftArm, Controls.leftArmPressed, holdData);
            
            CheckArm(_rightArm, Controls.rightArmPressed, holdData);

        }

        private void CheckArm(Arm arm, bool input, PlayerBrain.ClimbableData holdData)
        {
            if (input)
            {
                if(holdData.hold != null)
                    if (!arm.isAttached || _enabledBuild)
                    {
                        _enabledBuild = false;
                        ConnectArm(arm, holdData);
                        EnterNormalTime();
                    }
            }
            else
            {
                if (arm.isAttached &&  !_enabledBuild)
                {
                    DisconnectArm(arm);

                }
            }
        }

        private void OnAttemptMantle()
        {
            LedgeGrabResult result = CheckLedgeGrab();
            if (result.isValid)
            {
                Player.transform.DOMove(result.position + Data.ledgeGrabExitOffset, 0.5f);
                Player.ChangeToMovementState();
            }
            else
            {
                Jump();    
            }
        }

        private void Jump()
        {
            if (!_leftArm.isAttached && !_rightArm.isAttached) return;
            
            DisconnectLeftArm();
            DisconnectRightArm();

            Vector3 dir = Player.transform.right * Controls.moveInput.x + Player.transform.up * Controls.moveInput.y;
            if (dir.magnitude == 0)
            {
                dir = Player.transform.up;
            }
            Rb.AddForce(dir * Data.climbJumpStength, ForceMode.Impulse);
            
            //TODO time manager
            EnterSlowTime();
            
            Player.StartCoroutine(ResetTimeScale());
            IEnumerator ResetTimeScale()
            {
                yield return new WaitForSeconds(Data.climbJumpSlowdownLength);
                EnterNormalTime();
            }
        }

        private void EnterSlowTime()
        {
            Time.timeScale = Data.climbJumpTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        private void EnterNormalTime()
        {
            if (Time.timeScale == Data.climbJumpTimeScale)
            {
                Time.timeScale = 1.0f;

                Time.fixedDeltaTime = Time.timeScale * 0.02f;

            }
        }
        private LedgeGrabResult CheckLedgeGrab()
        {
            Vector3 ledgePos = Player.transform.TransformPoint(Data.ledgeGrabPos);

            if (Physics.Raycast(ledgePos, Vector3.down, out RaycastHit hit, Data.ledgeGrabDistance, Data.groundLayers))
            {
                return new LedgeGrabResult
                {
                    isValid = true,
                    position = hit.point
                };
            }

            return new LedgeGrabResult
            {
                isValid = false,
                position = Vector3.zero
            };
        }

        struct LedgeGrabResult
        {
            public bool isValid;
            public Vector3 position;
        }
    }
}