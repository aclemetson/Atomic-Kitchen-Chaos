using System;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects {
    [CreateAssetMenu()]
    [RequireComponent(typeof(NucleusGenerator))]
    [RequireComponent(typeof(ElectronGenerator))]
    public class AtomicObjectSO : ScriptableObject, ISettingsObject {
        public string displayName;
        public string atomicSymbol;
        public int protons;
        public int neutrons;
        public int electrons;
        public float generateTime;

        public string DisplayName => displayName;
        public int Nucleons => protons + neutrons;
        public string Ionization {
            get {
                if (protons > electrons) {
                    if (Math.Abs(protons - electrons) == 1) return "+";
                    return $"+{protons - electrons}";
                } else if (electrons > protons) {
                    if (Math.Abs(protons - electrons) == 1) return "-";
                    return (protons - electrons).ToString();
                } else {
                    return "";
                }
            }
        }
    }
}