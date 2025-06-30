using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.GeneratedObjects.ScriptableObjects {
    [CreateAssetMenu(menuName = "Generated Objects/ExoticMaterialSO")]
    public class ExoticMaterialSO : ScriptableObject {
        public enum ExoticMaterial {
            Positron,
            Neutrino,
            GammaRay
        }

        public ExoticMaterial materialName;
    }
}
