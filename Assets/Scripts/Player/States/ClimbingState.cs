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
        public ClimbingState(PlayerBrain player, PlayerControls controls, PlayerData data, Rigidbody rb, Arm leftArm, Arm rightArm) : base(player, controls, data, rb)
        {
            _leftArm = leftArm;
            _rightArm = rightArm;
        }

        public override void OnEnterState()
        {
            Controls.JumpPressed += OnAttemptMantle;
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
            CheckArmHolds();

            if (Player.GroundCheck() && !_leftArm.isAttached && !_rightArm.isAttached)
            {
                Player.ChangeToMovementState();
                return;
            }
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
                    if (!arm.isAttached)
                    {
                        ConnectArm(arm, hold);
                        EnterNormalTime();
                    }
            }
            else
            {
                if(arm.isAttached)
                    DisconnectArm(arm);
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
            
            Rb.AddForce(dir * Data.jumpStrength, ForceMode.Impulse);
            
            //TODO time manager
            Time.timeScale = 0.3f;
            Player.StartCoroutine(ResetTimeScale());
            IEnumerator ResetTimeScale()
            {
                yield return new WaitForSeconds(2f);
                EnterNormalTime();
            }
        }

        private void EnterNormalTime()
        {
            if (Time.timeScale == 0.3f) 
                Time.timeScale = 1.0f;
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