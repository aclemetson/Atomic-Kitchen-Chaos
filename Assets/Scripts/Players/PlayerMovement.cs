using AtomicKitchenChaos.InputActions;
using AtomicKitchenChaos.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AtomicKitchenChaos.Players {
    public class PlayerMovement : MonoBehaviour {

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private LayerMask groundMask;

        private PlayerInputActions inputActions;
        private Vector2 moveDirection;
        private Camera mainCamera;

        public bool CanMove => UIManager.Instance.MenuIsUp;

        private void Awake() {
            inputActions = new PlayerInputActions();
            mainCamera = Camera.main;

            inputActions.Player.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += _ => moveDirection = Vector2.zero;
        }

        private void OnEnable() {
            inputActions.Enable();
        }

        private void OnDisable() {
            inputActions.Disable();
        }

        // Update is called once per frame
        private void Update() {

            if (CanMove) {
                Vector3 inputDir = new Vector3(moveDirection.x, 0f, moveDirection.y).normalized;
                Vector3 movement = transform.rotation * inputDir;
                transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

                FaceMouse();
            }
        }

        private void FaceMouse() {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(transform.position, Vector3.forward * 2f, Color.red);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask)) {
                Vector3 lookAtPoint = hit.point;
                lookAtPoint.y = transform.position.y;
                Vector3 direction = lookAtPoint - transform.position;
                if (direction.sqrMagnitude > 0.01) {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
                }
            }
        }
    }
}