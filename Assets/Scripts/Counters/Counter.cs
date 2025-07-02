using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Players;
using AtomicKitchenChaos.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.Counters {
    public abstract class Counter : MonoBehaviour {

        protected static readonly string INTERACT_LAYER = "Interact";

        protected PlayerManager playerManager;
        protected bool isNextTo = false;
        protected CounterSO counterSO;
        private bool addedInteraction = false;

        protected abstract void Interact();
        protected abstract void SettingsInteraction();

        protected virtual void Start() {
            playerManager = PlayerManager.Instance;
            CustomizeVisual();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer(INTERACT_LAYER) && !isNextTo) {
                AddInteraction();
                isNextTo = true;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer(INTERACT_LAYER) && isNextTo) {
                RemoveInteraction();
                isNextTo = false;
            }
        }

        protected void AddInteraction() {
            if (!addedInteraction) {
                playerManager.PrimaryInteractionEvent.AddListener(Interact);
                playerManager.SecondaryInteractionEvent.AddListener(SettingsInteraction);
                addedInteraction = true;
            }
        }

        protected void RemoveInteraction() {
            if (addedInteraction) {
                playerManager.PrimaryInteractionEvent.RemoveListener(Interact);
                playerManager.SecondaryInteractionEvent.RemoveListener(SettingsInteraction);
                addedInteraction = false;
            }
        }

        public void SetCounterSO(CounterSO counterSO) {
            this.counterSO = counterSO;
        }

        protected void SetLabel(AtomicObjectSO atomicObjectSO, AtomLabelContainerUI atomLabelContainerUI) {
            atomLabelContainerUI.SetAtomPanel(atomicObjectSO);
        }

        protected void ClearLabel(AtomLabelContainerUI atomLabelContainerUI) {
            atomLabelContainerUI.ClearLabel();
        }

        [Serializable]
        protected struct HoldPosition {
            public Transform transform;
            public AtomLabelContainerUI atomLabelContainerUI;
        }

        private void CustomizeVisual() {
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            if(meshRenderer) {
                meshRenderer.material = counterSO.material;
            }
        }
    }
}