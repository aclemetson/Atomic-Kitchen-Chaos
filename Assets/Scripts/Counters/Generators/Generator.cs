using AtomicKitchenChaos.GeneratedObjects;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Generators {
    public class Generator : Counter {

        [SerializeField] private AtomicObjectSO atomicObjectSO;

        protected override void Interact() {
            if (state == State.Full && !playerManager.HasAtomicObject()) {
                TryCollect();
            }
        }

        protected override void FinishedWork() {
            state = State.Full;
            if (atomicObjectSO != null && atomicObjectSO.atomicObjectPrefab != null) {
                storedObject = Instantiate(atomicObjectSO.atomicObjectPrefab, holdPosition);
                storedObject.DisplayName = atomicObjectSO.displayName;
            }
            if (isInteracted) {
                AddInteraction();
            }
        }

        protected override void StartWork() {
            progressBar.SetFillTime(atomicObjectSO.generateTime);
        }

        private void Update() {
            if(state == State.Idle) {
                state = State.Working;
                StartWork();
            }
        }

        protected override void SettingsInteraction() {
            throw new System.NotImplementedException();
        }
    }
}
