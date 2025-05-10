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
        [SerializeField] private float comboBuffer = 0.6f;
        [SerializeField] private float[] animationDurations;
        // [SerializeField] private int maxAttackStage = 4;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private PlayerInputActions _playerInputActions;

        private static int inputXHash = Animator.StringToHash("InputX");
        private static int inputYHash = Animator.StringToHash("InputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static int isIdleDodgeHash = Animator.StringToHash("isIdleDodging");
        private static readonly int idleDodgeTriggerHash = Animator.StringToHash("IdleDodgeTrigger");

        private static int isPlayerActionHash = Animator.StringToHash("isPlayingAction"); 

        private static int heavyAttackTriggerHash = Animator.StringToHash("heavyAttack");

        private static int comboWindowOpenHash = Animator.StringToHash("canCombo");
        private static int attack1TriggerHash = Animator.StringToHash("attack1");
        private static int attack2TriggerHash = Animator.StringToHash("attack2");
        private static int attack3TriggerHash = Animator.StringToHash("attack3");
        private static int attack4TriggerHash = Animator.StringToHash("attack4");

        private static int attackStage;
        private static int comboStage;

        private Vector3 _currentBlendInput = Vector3.zero;

        private Coroutine openWindowCoroutine;
        private Coroutine closeWindowCoroutine;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _playerInputActions = GetComponent<PlayerInputActions>();
            attackStage = 0;
            comboStage = 0;
            animationDurations = new float[4];
            animationDurations[0] = 0.35f;
            animationDurations[1] = 0.50f;
            animationDurations[2] = 0.30f;
            animationDurations[3] = 0.75f;
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
            // _animator.SetBool(isHeavyAttackingHash, _playerInputActions);

            // kick
            if (_playerInputActions.HeavyAttackPressed)
            {
                _animator.SetTrigger(heavyAttackTriggerHash);
                _playerInputActions.ResetHeavyAttackPressed(); // reset
            }

            // basic sequence
            if (_playerInputActions.BasicAttackPressed)
            {
                _playerInputActions.ResetBasicAttackPressed(); // reset

                if (_animator.GetBool(comboWindowOpenHash))
                {
                    if (attackStage == 3 && comboStage == 2) 
                    {
                        _animator.SetTrigger(attack4TriggerHash);
                        attackStage = 0;
                        Debug.Log("Attack 4");
                    }
                    else if (attackStage == 2 && comboStage == 1) {
                        _animator.SetTrigger(attack3TriggerHash);
                        attackStage = 3;
                        Debug.Log("Attack 3");
                    }
                    else if (attackStage == 1 && comboStage == 0) {
                        _animator.SetTrigger(attack2TriggerHash);
                        attackStage = 2;
                        Debug.Log("Attack 2");
                    }
                    else if (comboStage == 3)
                    {
                        _animator.SetTrigger(attack1TriggerHash);
                        attackStage = 1;
                        Debug.Log("Attack 1");
                    }
                    // attackStage = (attackStage + 1) % 4;
                }
                else
                {
                    _animator.SetTrigger(attack1TriggerHash);
                    attackStage = 1;
                    Debug.Log("Attack 1 - Default");
                }
            }

            _animator.SetBool(isPlayerActionHash, isHeavy); 
        }

        public void OpenComboWindow(int stage)
        {
            if (openWindowCoroutine != null) StopCoroutine(openWindowCoroutine);

            openWindowCoroutine = StartCoroutine(OpenComboWindowBuffered(stage));
        }

        private IEnumerator OpenComboWindowBuffered(int stage)
        {
            _animator.SetBool(comboWindowOpenHash, true);
            Debug.Log("Combo window opened in stage " + stage);

            comboStage = stage;

            yield return new WaitForSeconds(animationDurations[stage] + 0.95f);

            _animator.SetBool(comboWindowOpenHash, false);
            Debug.Log("Combo window closed from stage " + stage);
        }
    }
}