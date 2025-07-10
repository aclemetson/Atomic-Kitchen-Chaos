using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AtomicKitchenChaos.InputActions {
    public static class AnyKeyInput {
        private static InputAction anyKeyAction;
        private static bool inputReceived, canReceiveInput = false;

        private static UnityEvent anyKeyPressedEvent;

        private static int numListeners = 0;

        public static void Init() {
            if (anyKeyAction != null)
                return;

            anyKeyAction = new InputAction(
                type: InputActionType.PassThrough,
                binding: "<Keyboard>/anyKey"
            );

            // Game Pads
            anyKeyAction.AddBinding("<Gamepad>/buttonSouth");
            anyKeyAction.AddBinding("<Gamepad>/start");
            anyKeyAction.AddBinding("<Gamepad>/select");

            // Mouse Clicks
            anyKeyAction.AddBinding("<Mouse>/leftButton");
            anyKeyAction.AddBinding("<Mouse>/rightButton");

            anyKeyAction.performed += OnAnyKeyPressed;
            anyKeyAction.Enable();

            anyKeyPressedEvent = new UnityEvent();

            canReceiveInput = true;
            numListeners = 0;
        }

        private static void OnAnyKeyPressed(InputAction.CallbackContext context) {
            if (!canReceiveInput || inputReceived)
                return;

            inputReceived = true;
            canReceiveInput = false;

            if (numListeners > 0) {
                anyKeyPressedEvent?.Invoke();
                anyKeyPressedEvent.RemoveAllListeners();

                anyKeyAction.Disable();
            } else {
                inputReceived = false;
                canReceiveInput = true;
            }
        }

        public static void AddOnAnyKeyPressed(UnityAction action) {
            anyKeyPressedEvent.AddListener(action);
            numListeners++;
        }

        public static void Reset() {
            inputReceived = false;
            canReceiveInput = true;
            anyKeyAction?.Enable();
            numListeners = 0;
        }

        public static void Dispose() {
            anyKeyAction?.Disable();
            anyKeyAction?.Dispose();
            anyKeyAction = null;

            anyKeyPressedEvent?.RemoveAllListeners();
            anyKeyPressedEvent = null;

            inputReceived = false;
            canReceiveInput = false;
            numListeners = 0;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnEnterPlayMode]
        static void OnEnterPlayMode() {
            Dispose();
        }
#endif
    }
}