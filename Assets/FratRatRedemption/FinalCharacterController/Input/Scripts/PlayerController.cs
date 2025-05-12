using FratRatRedemption.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FratRatRedemption.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;

        [Header("Base Movement")]
        public float runAcceleration = 35f;
        public float runSpeed = 4f;
        public float sprintAcceleration = 50f;
        public float sprintSpeed = 7f;
        public float drag = 20f;
        public float movingThreshold = 0.01f;
        public float gravity = 25f;
        private float _verticalVelocity = 0f;

        [Header("Animation")]
        public float playerModelRotationSpeed = 10f;
        public float rotateToTargetTime = 0.67f;


        [Header("Dodge")]
        public float dodgeAcceleration = 1000f;
        public float dodgeSpeed = 24f;
        public float dodgeDuration = 0.4f;
        private float _dodgeTimer = 0f;
        public const float _dodgeDuration = 0.4f;
        private float _lastDodgeTime = float.NegativeInfinity; // So dodge is available at game start
        public  float _dodgeCooldown = 0.06f;     // 60 milliseconds
        private bool dodgeInitiated = false;
        private int _consecutiveDodges = 0;
        private float _lastConsecutiveDodgeTime = float.NegativeInfinity;
        public  float _consecutiveWindow = 1.0f; // Time window to reset combo
        public  float _consecutiveDodgeCooldown = 0.8f; // ZZZ-style cooldown
        private Vector3 _dodgeDirection;

        [Header("Heavy")]
        


        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;
        #endregion

        #region Startup
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            UpdateMovementState();

            Vector3 lateralMove = HandleLateralMovement();
            float verticalMove = HandleVerticalMovement();

            Vector3 finalMove = new Vector3(lateralMove.x, verticalMove, lateralMove.z);
            _characterController.Move(finalMove * Time.deltaTime);
        }


        private void UpdateMovementState()
        {
            bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
            bool isMovingLaterally = IsMovingLaterally();
            bool wantsToDodge = _playerLocomotionInput.DodgePressed;
            bool canDodge = Time.time - _lastDodgeTime >= _dodgeCooldown;
            bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
            bool isIdleDodgeTriggered = false;


            PlayerMovementState lateralState;

            // Start dodge if allowed
            // Reset combo if window expired
            if (Time.time - _lastConsecutiveDodgeTime > _consecutiveWindow)
            {
                _consecutiveDodges = 0;
                //Debug.Log("Consecutive dodges reset (recharged).");
            }

            bool dodgeCooldownExpired = _consecutiveDodges < 2 || Time.time - _lastConsecutiveDodgeTime >= _consecutiveDodgeCooldown;

            if (wantsToDodge && canDodge && dodgeCooldownExpired && !dodgeInitiated)
            {
                _lastDodgeTime = Time.time;
                _lastConsecutiveDodgeTime = Time.time;
                _consecutiveDodges++;
                dodgeInitiated = true;

              //  Debug.Log($"Dodge used: {_consecutiveDodges}/2");

                Vector2 input = _playerLocomotionInput.MovementInput;
                
                if (input == Vector2.zero)
                {
                    //print(input);
                    lateralState = PlayerMovementState.IdleDodge;
                    isIdleDodgeTriggered = true;
                    //print("State changed to idleDodge!"); 
                    // print(lateralState);
                    // Set dodge direction to backward
                    Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
                    _dodgeDirection = -cameraForwardXZ;
                }
                else
                {
                    lateralState = PlayerMovementState.Dodging;

                    // Set dodge direction based on input
                    Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
                    Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
                    _dodgeDirection = (cameraRightXZ * input.x + cameraForwardXZ * input.y).normalized;
                }
            }
            else if (wantsToDodge && !dodgeCooldownExpired)
            {
               // Debug.Log("Dodge attempt denied: cooldown active after 2 consecutive dodges.");
            }



            // Determine time since last dodge
            float timeSinceDodge = Time.time - _lastDodgeTime;


            // If we are in the dodge window
            if (timeSinceDodge < dodgeDuration)
            {
                if (isIdleDodgeTriggered || _playerState.CurrentPlayerMovementState == PlayerMovementState.IdleDodge)
                {
                    lateralState = PlayerMovementState.IdleDodge;
                    //print(lateralState);
                }
                else
                {
                    lateralState = PlayerMovementState.Dodging;
                }
            }

            else if (isSprinting)
            {
                lateralState = PlayerMovementState.Sprinting;
            }
            else if (isMovingLaterally || isMovementInput)
            {
                lateralState = PlayerMovementState.Running;
                dodgeInitiated = false;
            }
            else
            {
                lateralState = PlayerMovementState.Idling;
                dodgeInitiated = false;
            }
            _playerState.SetPlayerMovementState(lateralState);
            _playerLocomotionInput.DodgePressed = false;


        }
        private float HandleVerticalMovement()
        {
            if (_characterController.isGrounded)
            {
                _verticalVelocity = -1f;
            }
            else
            {
                _verticalVelocity -= gravity * Time.deltaTime;
            }

            return _verticalVelocity;
        }


        private Vector3 HandleLateralMovement()
        {
            PlayerMovementState currentState = _playerState.CurrentPlayerMovementState;

            float lateralAcceleration;
            float clampLateralMagnitude;

            switch (currentState)
            {
                case PlayerMovementState.Dodging:
                case PlayerMovementState.IdleDodge: // ✅ include this!
                    lateralAcceleration = dodgeAcceleration;
                    clampLateralMagnitude = dodgeSpeed;
                    break;
                case PlayerMovementState.Sprinting:
                    lateralAcceleration = sprintAcceleration;
                    clampLateralMagnitude = sprintSpeed;
                    break;
                case PlayerMovementState.Running:
                    lateralAcceleration = runAcceleration;
                    clampLateralMagnitude = runSpeed;
                    break;
                default:
                    lateralAcceleration = 0f;
                    clampLateralMagnitude = 0f;
                    break;
            }


            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;

            Vector3 movementDirection = (currentState == PlayerMovementState.Dodging || currentState == PlayerMovementState.IdleDodge)
                 ? _dodgeDirection
                 : cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);

            return new Vector3(newVelocity.x, 0f, newVelocity.z);
        }

        #endregion

        #region Late Update Logic
        private void LateUpdate()
        {
            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
        }
        #endregion

        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.y);

            return lateralVelocity.magnitude > movingThreshold;
        }
        #endregion
    }
}