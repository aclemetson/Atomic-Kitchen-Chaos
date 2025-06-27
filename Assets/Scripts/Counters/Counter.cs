using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.Players;
using AtomicKitchenChaos.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace AtomicKitchenChaos.Counters {
    public abstract class Counter : MonoBehaviour {

        protected static readonly string INTERACT_LAYER = "Interact";

        protected PlayerManager playerManager;
        protected bool isNextTo = false;
        private bool addedInteraction = false;

        protected abstract void Interact();
        protected abstract void SettingsInteraction();

        protected virtual void Start() {
            playerManager = PlayerManager.Instance;
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
    }
}