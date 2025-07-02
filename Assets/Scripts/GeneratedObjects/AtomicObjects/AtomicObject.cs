using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects.AtomicObjects {
    public class AtomicObject : MonoBehaviour {
        private AtomicObjectSO atomicObjectSO;

        public AtomicObjectSO AtomicObjectSO => atomicObjectSO;

        public void SetAtomicObjectSO(AtomicObjectSO so) {
            atomicObjectSO = so;
            GetComponent<NucleusGenerator>().GenerateNucleus(atomicObjectSO.protons, atomicObjectSO.neutrons);
            GetComponent<ElectronGenerator>().GenerateElectrons(atomicObjectSO.electrons);
        }
    }
}
