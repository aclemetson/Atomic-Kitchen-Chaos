using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.Players;
using AtomicKitchenChaos.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.Counters {
    public abstract class Counter : MonoBehaviour {

        protected enum State {
            Idle,
            Working,
            Full,
        }

        protected static readonly string INTERACT_LAYER = "Interact";

        [SerializeField] protected Transform holdPosition;
        [SerializeField] protected ProgressBarUI progressBar;

        protected PlayerManager playerManager;
        protected bool isInteracted = false;
        protected State state = State.Idle;
        protected AtomicObject storedObject;


        protected virtual void Start() {
            playerManager = PlayerManager.Instance;
            progressBar.OnTimeElapsed.AddListener(FinishedWork);
        }

        protected abstract void Interact();
        protected abstract void SettingsInteraction();
        protected abstract void StartWork();
        protected abstract void FinishedWork();

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer(INTERACT_LAYER) && !isInteracted) {
                AddInteraction();
                isInteracted = true;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer(INTERACT_LAYER) && isInteracted) {
                RemoveInteraction();
                isInteracted = false;
            }
        }

        protected void AddInteraction() {
            playerManager.PrimaryInteractionEvent.AddListener(Interact);
            playerManager.SecondaryInteractionEvent.AddListener(SettingsInteraction);
        }

        protected void RemoveInteraction() {
            playerManager.PrimaryInteractionEvent.RemoveListener(Interact);
            playerManager.SecondaryInteractionEvent.RemoveListener(SettingsInteraction);
        }

        protected void TryCollect() {
            if (!playerManager.HasAtomicObject()) {
                playerManager.SetAtomicObject(storedObject);
                storedObject = null;
                state = State.Idle;
            }
        }
    }
}