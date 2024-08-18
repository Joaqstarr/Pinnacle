using System;
using Player.States;
using UnityEngine;

namespace Player
{
    public class PlayerBrain : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;
        private PlayerControls _controls;
        private Rigidbody _rb;

        [field: Header("Body References")]
        [field: SerializeField]public Transform headPos { get; private set; }
        [field: SerializeField]public Arm leftArm { get; private set; }
        [field: SerializeField]public Arm rightArm { get; private set; }
        
        //state stuff 
        private BaseState _currentState;
        //States
        private BaseState _movementState;
        private BaseState _climbState;


        [SerializeField] private bool _drawGizmos = true;
        private void Awake()
        {
            _controls = GetComponent<PlayerControls>();
            _rb = GetComponent<Rigidbody>();
            
            //State setup
            _movementState = new MovementState(this, _controls, _data,_rb);
            _climbState = new ClimbingState(this, _controls, _data, _rb,leftArm, rightArm);
        }

        // Start is called before the first frame update
        void Start()
        {
            

            ChangeToMovementState();
            
        }

        // Update is called once per frame
        void Update()
        {
            _currentState.OnUpdateState();
        }

        private void FixedUpdate()
        {
            _currentState.OnFixedUpdateState();
        }

        public void ChangeToState(BaseState newState)
        {
            if(_currentState != null)
                _currentState.OnExitState();

            _currentState = newState;
            
            _currentState.OnEnterState();
        }

        public void ChangeToMovementState()
        {
            ChangeToState(_movementState);
        }
        public void ChangeToClimbingState()
        {
            ChangeToState(_climbState);
        }
        private void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(transform.position + _data.groundCheckPos, 0.1f);
            
            Gizmos.color = Color.red;
            Vector3 ledgePos = transform.TransformPoint(_data.ledgeGrabPos);
            Gizmos.DrawSphere(ledgePos, 0.1f);
            Gizmos.DrawRay(ledgePos, Vector3.down * _data.ledgeGrabDistance);
            
        }

        public Climbable CheckForClimbable()
        {
            Debug.DrawRay(headPos.position, headPos.forward, Color.cyan);
            if (!Physics.Raycast(headPos.position, headPos.forward, out RaycastHit hit, _data.maxGrabHoldDistance,
                    _data.climbLayers))
            {
                

                return null;
            }
            Climbable hold = hit.transform.GetComponent<Climbable>();
            return hold;
        }


        public void CameraMovement()
        {
            LookX();
            LookY();
        }
        private void LookX()
        {
            Vector3 curRot = transform.eulerAngles;
            curRot.y += _data.lookSpeed * _controls.lookInput.x;
            transform.eulerAngles = curRot;
        }

        private void LookY()
        {
            Vector3 curRot = headPos.eulerAngles;
            curRot.x += _data.lookSpeed * _controls.lookInput.y;
            if(curRot.x > 180)
            {
                curRot.x = Mathf.Clamp(curRot.x, 270f, 361f);

            }
            else
            {
                curRot.x = Mathf.Clamp(curRot.x, -1, 90f);

            }
            headPos.eulerAngles = curRot;

        }
        
        public bool GroundCheck()
        {
            Collider[] result = Physics.OverlapSphere(transform.position + _data.groundCheckPos, 0.1f, _data.groundLayers);
            if (result.Length == null) return false;

            if (result.Length == 0) return false;
            return true;

        }
    }
}

