using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FratRatRedemption.FinalCharacterController
{
    public class PlayerState : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;

        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
        }
    }
    public enum PlayerMovementState
    {
        Idling = 0,
        Walking = 1,
        Running = 2,
        Dodging = 3,
        Sprinting = 4,
        IdleDodge = 5,
        Heavy = 6
    }
}