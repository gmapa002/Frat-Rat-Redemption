using FratRatRedemption.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FratRatRedemption.FinalCharacterController
{
    [DefaultExecutionOrder(-100)]
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;
        public PlayerControls PlayerControls { get; private set; }

        private void Awake()
        {            
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            PlayerControls = new PlayerControls();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            PlayerControls?.Enable();
        }

        private void OnDisable()
        {
            PlayerControls?.Disable();
        }
    }
}