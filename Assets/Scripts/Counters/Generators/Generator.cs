using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Generators {
    public class Generator : Worker {



        private GeneratorSO generatorSO;

        protected override void Start() {
            base.Start();

            generatorSO = (GeneratorSO)counterSO;

            // Clear all Labels
            foreach(var item in holdPositions) {
                ClearLabel(item.atomLabelContainerUI);
            }

            // Customize the Visual
            CustomizeVisual();

            // Start Generating
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
            if (generatorSO.atomicObjectSO != null && atomPrefab != null) {
                var go = Instantiate(atomPrefab, holdPositions[0].transform);
                SetAtomicObject(go, generatorSO.atomicObjectSO);
                SetLabel(generatorSO.atomicObjectSO, holdPositions[0].atomLabelContainerUI);
            }
            if (isNextTo) {
                AddInteraction();
            }
        }

        protected override void StartWork() {
            progressBar.SetFillTime(generatorSO.atomicObjectSO.generateTime);
        }

        protected override void SettingsInteraction() {
        }
    }
}
