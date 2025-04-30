using FratRatRedemption.FinalCharacterController;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FratRatRedemption.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerInputActions : MonoBehaviour, PlayerControls.IPlayerActionMapActions
    {
        #region classVariables
        public bool BasicAttackPressed { get; private set; }
        public bool HeavyAttackPressed {  get; private set; }
        #endregion

        private void OnEnable()
        {
            //PlayerControls = new PlayerControls();
            //PlayerInputManager.Instance.Enable();

 


            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.SetCallbacks(this);

        }

        private void OnDisable()
        {
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.RemoveCallbacks(this);
        }

        public void OnNewaction(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }


        public void OnBasicAttack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            BasicAttackPressed = true;
        }

        public void OnHeavyAttack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            HeavyAttackPressed = true;
        }
    }
}