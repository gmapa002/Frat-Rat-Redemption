using FratRatRedemption.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FratRatRedemption.FinalCharacterController.PlayerState;

namespace FratRatRedemption.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private PlayerInputActions _playerInputActions;

        private static int inputXHash = Animator.StringToHash("InputX");
        private static int inputYHash = Animator.StringToHash("InputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static int isIdleDodgeHash = Animator.StringToHash("isIdleDodging");
        private static readonly int idleDodgeTriggerHash = Animator.StringToHash("IdleDodgeTrigger");

        private static int isHeavyAttackingHash = Animator.StringToHash("isHeavyAttack");
        private static int isPlayerActionHash = Animator.StringToHash("isPlayingAction"); 
            


        private Vector3 _currentBlendInput = Vector3.zero;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isDodging = _playerState.CurrentPlayerMovementState == PlayerMovementState.Dodging;
            bool isIdleDodging = _playerState.CurrentPlayerMovementState == PlayerMovementState.IdleDodge;
            bool isHeavy = _playerState.CurrentPlayerMovementState == PlayerMovementState.Heavy; 





            Vector2 inputTarget = isDodging ? _playerLocomotionInput.MovementInput * 10f : _playerLocomotionInput.MovementInput * 5f ;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);



            _animator.SetBool(isIdleDodgeHash, isIdleDodging);
            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _animator.SetBool(isHeavyAttackingHash, _playerInputActions);
            _animator.SetBool(isPlayerActionHash, isHeavy); 
        }
    }
}