using AtomicKitchenChaos.GeneratedObjects;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Generators {
    public class Generator : Worker {

        [SerializeField] private AtomicObjectSO atomicObjectSO;

        protected override void Start() {
            base.Start();
            StartWork();
            state = State.Working;
        }

        protected override void Interact() {
            if (state == State.Full && !playerManager.HasAtomicObject()) {
                if (TryCollect()) {
                    StartWork();
                    state = State.Working;
                }
            }
        }

        protected override void FinishedWork() {
            state = State.Full;
            if (atomicObjectSO != null && atomicObjectSO.atomicObjectPrefab != null) {
                storedObject = Instantiate(atomicObjectSO.atomicObjectPrefab, holdPositions[0]);
                storedObject.DisplayName = atomicObjectSO.displayName;
            }
            if (isNextTo) {
                AddInteraction();
            }
        }

        protected override void StartWork() {
            progressBar.SetFillTime(atomicObjectSO.generateTime);
        }


        protected override void SettingsInteraction() {

        }
    }
}
