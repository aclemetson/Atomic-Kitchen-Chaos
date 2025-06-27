using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.GeneratedObjects {
    [CreateAssetMenu()]
    public class ExoticMaterialSO : ScriptableObject {
        public enum ExoticMaterial {
            Positron,
            Neutrino,
            GammaRay
        }

        public ExoticMaterial materialName;
    }
}
