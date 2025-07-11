using UnityEngine;

namespace AtomicKitchenChaos.Camera {
    public class LookAtCamera : MonoBehaviour {
        private enum Mode {
            LookAt,
            LookAtInverted,
            CameraForward,
            CameraForwardInverted,
        }

        [SerializeField] private Mode mode;

        private void LateUpdate() {
            switch (mode) {
                case Mode.LookAt:
                    transform.LookAt(UnityEngine.Camera.main.transform);
                    break;

                case Mode.LookAtInverted:
                    Vector3 dirFromCamera = transform.position - UnityEngine.Camera.main.transform.forward;
                    transform.LookAt(transform.position + dirFromCamera);
                    break;

                case Mode.CameraForward:
                    transform.forward = UnityEngine.Camera.main.transform.forward;
                    break;

                case Mode.CameraForwardInverted:
                    transform.forward = -UnityEngine.Camera.main.transform.forward;
                    break;
            }
        }
    }
}
