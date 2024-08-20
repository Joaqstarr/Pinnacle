using System;
using Objects.ObjectTypes;
using Player.States;
using Unity.VisualScripting;
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
        private PlaceObject _buildSystem ;
        [SerializeField]
        private AudioSource _jumpSource;

        private Vector3 _setCheckpoint;
        
        //state stuff 
        private BaseState _currentState;
        //States
        private BaseState _movementState;
        private BaseState _climbState;

        
        public bool isBuildModeEnabled{ get; private set; } = false;
        
        [SerializeField] private bool _drawGizmos = true;
        private void Awake()
        {

            _controls = GetComponent<PlayerControls>();
            _rb = GetComponent<Rigidbody>();
            _buildSystem = GetComponent<PlaceObject>();
            _buildSystem.Initialize(_data, _controls,headPos);
            //State setup
            _movementState = new MovementState(this, _controls, _data,_rb);
            _climbState = new ClimbingState(this, _controls, _data, _rb,leftArm, rightArm);
        }

        // Start is called before the first frame update
        void Start()
        {
            _setCheckpoint = GetCheckpoint();
            transform.position = _setCheckpoint;

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

        public struct ClimbableData
        {
            public ClimbableData(Climbable newHold, Vector3 grabPos, Vector3 normal)
            {
                this.hold = newHold;
                this.grabPosition = grabPos;
                normalDir = normal;
            }

            public Climbable hold { get; }
            public Vector3 grabPosition { get; }
            public Vector3 normalDir { get; }

        }
        public ClimbableData CheckForClimbable()
        {
            //Debug.DrawRay(headPos.position, headPos.forward, Color.cyan);
            if (!Physics.Raycast(headPos.position, headPos.forward, out RaycastHit hit, _data.maxGrabHoldDistance,
                    _data.climbLayers))
            {
                

                return new ClimbableData(null, Vector3.zero, Vector3.one);
            }
            Climbable hold = hit.transform.GetComponent<Climbable>();
            return new ClimbableData(hold, hit.point,hit.normal);
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

            if (result.Length == 0) return false;
            return true;

        }

        private void ToggleBuildMode()
        {
            if (isBuildModeEnabled)
            {
                DisableBuildMode();
            }
            else
            {
                EnableBuildMode();
            }
        }

        private void EnableBuildMode()
        {
            isBuildModeEnabled = true;
            _currentState.OnEnterBuildMode();
            _buildSystem.EnterBuildMode();
        }
        private void DisableBuildMode()
        {
            isBuildModeEnabled = false;
            _currentState.OnExitBuildMode();
            _buildSystem.ExitBuildMode();
        }
        private void OnEnable()
        {
            Checkpoint.NewCheckpointSet += SetCheckpoint;
            _controls.ResetPressed += ReturnToCheckpoint;

            _controls.BuildPressed += ToggleBuildMode;
        }

        private void OnDisable()
        {
            Checkpoint.NewCheckpointSet -= SetCheckpoint;
            _controls.BuildPressed -= ToggleBuildMode;
            _controls.ResetPressed -= ReturnToCheckpoint;
        }

        private void ReturnToCheckpoint()
        {
            if (isBuildModeEnabled) return;

            ChangeToMovementState();
            transform.position = _setCheckpoint;
        }
        public void SetCheckpoint(Vector3 pos)
        {
            _setCheckpoint = pos;
            PlayerPrefs.SetFloat("checkX", pos.x);
            PlayerPrefs.SetFloat("checkY", pos.y);
            PlayerPrefs.SetFloat("checkZ", pos.z);

        }

        private Vector3 GetCheckpoint()
        {
            float x = PlayerPrefs.GetFloat("checkX", transform.position.x);
            float y = PlayerPrefs.GetFloat("checkY", transform.position.y);
            float z = PlayerPrefs.GetFloat("checkZ", transform.position.z);

            return new Vector3(x, y, z);

        }
        
        public void PlayJumpSound()
        {
            _jumpSource.Play();
        }

        public bool isClimbing
        {
            get
            {
                return _currentState == _climbState;
            }
        }
        
        
    }
}

