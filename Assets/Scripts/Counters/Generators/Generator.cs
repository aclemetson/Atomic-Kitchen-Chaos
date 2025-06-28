using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.UI;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Generators {
    public class Generator : Worker {

        [SerializeField] private AtomicObjectSO atomicObjectSO;

        protected override void Start() {
            base.Start();
            foreach(var item in holdPositions) {
                ClearLabel(item.atomLabelContainerUI);
            }
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
            if (atomicObjectSO != null && atomPrefab != null) {
                var go = Instantiate(atomPrefab, holdPositions[0].transform);
                SetAtomicObject(go, atomicObjectSO);
                SetLabel(atomicObjectSO, holdPositions[0].atomLabelContainerUI);
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
