using UnityEngine;

namespace Player
{
    [CreateAssetMenu( menuName = "PlayerData", order = 0)]
    public class PlayerData : ScriptableObject
    {
        [field: Header("Movement")]
        [field: SerializeField]
        public float moveSpeed { get; private set; } = 5;

        [field: SerializeField] public float maxForce { get; private set; } = 1;

        [field: SerializeField] public float lookSpeed { get; private set; } = 1;
        [field: SerializeField] public float gravity { get; private set; } = -10;
        [field: SerializeField] public float bonusGravitySpeed { get; private set; } = 2;

        [field: Header("Jump")]
        [field: SerializeField]public float jumpStrength { get; private set; } = 10;
        [field: SerializeField]public float airSpeedModifier { get; private set; } = 0.1f;
       
        [field: Header("Ground Check")]
        [field: SerializeField] public Vector3 groundCheckPos { get; private set; }
        [field: SerializeField] public LayerMask groundLayers { get; private set; }

        [field: Header("Climbing")]
        [field: SerializeField]
        public float maxGrabHoldDistance { get; private set; } = 2;
        [field: SerializeField] public LayerMask climbLayers { get; private set; }


    }
}