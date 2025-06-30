using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects.AtomicObjects {
    public class ElectronOrbit : MonoBehaviour {
        [SerializeField] private float orbitSpeed = 720f;
        [SerializeField] private float precessionSpeed = 10f;

        private Transform nucleus;
        private Vector3 orbitAxis;
        private Vector3 orbitStartOffset;
        private float orbitRadius = .6f;

        private void Start() {
            if (nucleus != null) {
                orbitAxis = Random.onUnitSphere;
                Vector3 randomDirection;
                do {
                    randomDirection = Random.onUnitSphere;
                } while (Vector3.Dot(randomDirection, orbitAxis) > 0.98f);

                orbitStartOffset = Vector3.Cross(orbitAxis, randomDirection).normalized * orbitRadius;
                transform.position = nucleus.position + orbitStartOffset;
            }
        }

        private void Update() {
            if (nucleus != null) {
                // Slowly change the orbit axis over time (simulate precession)
                Quaternion precession = Quaternion.AngleAxis(precessionSpeed * Time.deltaTime, Random.onUnitSphere);
                orbitAxis = precession * orbitAxis;
                orbitAxis.Normalize();

                transform.RotateAround(nucleus.position, orbitAxis, orbitSpeed * Time.deltaTime);
            }
        }

        internal void SetOrbitRadius(float orbitRadius) {
            this.orbitRadius = orbitRadius;
        }

        internal void SetNucleusLocation(Transform nucleus) {
            this.nucleus = nucleus;
        }
    }
}
