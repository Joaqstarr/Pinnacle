using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Player
{
    public class PlayerControls : MonoBehaviour
    {

        public static PlayerControls Instance;
        
        public Vector2 moveInput { get; private set; }
        public Vector2 lookInput{ get; private set; }
        
        public bool rightArmPressed{ get; private set; }
        public bool leftArmPressed{ get; private set; }

        //events
        public delegate void ButtonPressed();

        public ButtonPressed JumpPressed;

        
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
            
        }
        public void OnJump(InputValue value)
        {
            JumpPressed?.Invoke();
        }

        public void OnLeftArm(InputValue value)
        {
            leftArmPressed = value.isPressed;
        }
        public void OnRightArm(InputValue value)
        {
            rightArmPressed = value.isPressed;
        }
        private void Start()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}