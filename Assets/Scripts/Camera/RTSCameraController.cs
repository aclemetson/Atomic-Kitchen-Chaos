using AtomicKitchenChaos.Grid;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AtomicKitchenChaos.Camera
{
    public class RTSCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook virtualCamera;
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float zoomSpeed = 1000f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private Vector2 zoomRange = new Vector2(10f, 60f);

        private Transform camTransform;
        private Vector2 moveDirection;
        private float zoomValue;
        private float rotationValue;

        private void Start() {
            camTransform = virtualCamera.VirtualCameraGameObject.transform;

            BuildManager.Instance.InputActions.Builder.MoveCamera.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
            BuildManager.Instance.InputActions.Builder.MoveCamera.canceled += _ => moveDirection = Vector2.zero;
            BuildManager.Instance.InputActions.Builder.ZoomCamera.performed += ctx => zoomValue = ctx.ReadValue<float>();
            BuildManager.Instance.InputActions.Builder.ZoomCamera.canceled += _ => zoomValue = 0f;
            BuildManager.Instance.InputActions.Builder.RotateCamera.performed += ctx => rotationValue = ctx.ReadValue<float>();
            BuildManager.Instance.InputActions.Builder.RotateCamera.canceled += _ => rotationValue = 0f;
        }

        private void Update() {
            HandlePan();
            HandleZoom();
            HandleRotation();
        }

        private void HandlePan() {
            Vector3 inputDir = new Vector3(moveDirection.x, 0f, moveDirection.y).normalized;
            Vector3 movement = inputDir * panSpeed * Time.deltaTime;
            camTransform.position = new Vector3(camTransform.position.x + movement.x, camTransform.position.y, camTransform.position.z + movement.z);
        }

        private void HandleZoom() {
            if (!Mathf.Approximately(zoomValue, 0f)) {
                float currentHeight = camTransform.position.y;
                float newHeight = Mathf.Clamp(currentHeight - zoomValue * zoomSpeed * Time.deltaTime, zoomRange.x, zoomRange.y);
                camTransform.position = new Vector3(camTransform.position.x, newHeight, camTransform.position.z);
            }
        }

        private void HandleRotation() {
            if (!Mathf.Approximately(rotationValue, 0f)) {
                camTransform.RotateAround(camTransform.position, Vector3.up, rotationValue * rotationSpeed * Time.deltaTime);
            }
        }
    }
}
