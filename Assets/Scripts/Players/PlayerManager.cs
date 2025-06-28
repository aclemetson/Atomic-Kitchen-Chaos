using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.InputActions;
using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.Players {
    public class PlayerManager : MonoBehaviour {

        public static PlayerManager Instance;

        [SerializeField] private Transform interactionPosition;
        [SerializeField] private Transform holdObjectPosition;

        private PlayerInputActions inputActions;
        private AtomicObject atomicObject;
        private UnityEvent primaryInteractionEvent;
        private UnityEvent secondaryInteractionEvent;

        public Transform HoldObjectPosition => holdObjectPosition;
        public Transform InteractionPosition => interactionPosition;
        public AtomicObject AtomicObject => atomicObject;
        public UnityEvent PrimaryInteractionEvent => primaryInteractionEvent;
        public UnityEvent SecondaryInteractionEvent => secondaryInteractionEvent;


        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }

            inputActions = new PlayerInputActions();
            primaryInteractionEvent = new UnityEvent();
            secondaryInteractionEvent = new UnityEvent();
            inputActions.Player.Interact.performed += ctx => primaryInteractionEvent.Invoke();
            inputActions.Player.Settings.performed += ctx => secondaryInteractionEvent.Invoke();
        }

        public bool HasAtomicObject() {
            return atomicObject != null;
        }

        public void SetAtomicObject(AtomicObject gameObject) {
            if (atomicObject == null) {
                atomicObject = gameObject;
                atomicObject.gameObject.transform.SetParent(holdObjectPosition, false);
            }
        }

        public void RemoveAtomicObject(bool destroy = true) {
            if(destroy)
                Destroy(atomicObject.gameObject);
            atomicObject = null;
        }

        private void OnEnable() {
            inputActions.Enable();
        }

        private void OnDisable() {
            inputActions.Disable();
        }
    }
}
