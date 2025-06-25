using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects {
    [CreateAssetMenu()]
    public class AtomicObjectSO : ScriptableObject, ISettingsObject {
        public string displayName;
        public AtomicObject atomicObjectPrefab;
        public float generateTime;

        public string DisplayName  => displayName;
    }
}