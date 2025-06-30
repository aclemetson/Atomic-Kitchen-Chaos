using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.GeneratedObjects.AtomicObjects;
using AtomicKitchenChaos.UI;
using UnityEngine;

namespace AtomicKitchenChaos.Counters
{
    public class HoldCounter : Counter {

        [SerializeField] private HoldPosition[] holdPositions;

        protected override void Start() {
            base.Start();
            foreach (var item in holdPositions) {
                ClearLabel(item.atomLabelContainerUI);
            }
        }

        protected override void Interact() {
            if (!playerManager.HasAtomicObject()) {
                // Grab the object that is closest to the player
                AtomicObject closest = null;
                AtomLabelContainerUI atomLabelContainerUI = null;
                float closestDistanceSqr = Mathf.Infinity;

                foreach (var item in holdPositions) {
                    AtomicObject atomic = item.transform.GetComponentInChildren<AtomicObject>();
                    if (atomic != null) {
                        float distSqr = (atomic.transform.position - playerManager.transform.position).sqrMagnitude;
                        if (distSqr < closestDistanceSqr) {
                            closestDistanceSqr = distSqr;
                            closest = atomic;
                            atomLabelContainerUI = item.atomLabelContainerUI;
                        }
                    }
                }

                if (closest != null) {
                    // There is an object to grab
                    playerManager.SetAtomicObject(closest);
                    ClearLabel(atomLabelContainerUI);
                }
            } else {
                // Try to place an object
                Transform closest = null;
                AtomLabelContainerUI atomLabelContainerUI = null;
                float closestDistanceSqr = Mathf.Infinity;

                foreach (var item in holdPositions) {
                    AtomicObject atomic = item.transform.GetComponentInChildren<AtomicObject>();
                    if (atomic == null) {
                        float distSqr = (item.transform.position - playerManager.transform.position).sqrMagnitude;
                        if (distSqr < closestDistanceSqr) {
                            closestDistanceSqr = distSqr;
                            closest = item.transform;
                            atomLabelContainerUI = item.atomLabelContainerUI;
                        }
                    }
                }

                if (closest != null) {
                    AtomicObjectSO atomicObjectSO = playerManager.AtomicObject.atomicObjectSO;
                    playerManager.AtomicObject.transform.SetParent(closest, false);
                    playerManager.RemoveAtomicObject(false);
                    SetLabel(atomicObjectSO, atomLabelContainerUI);
                } else {
                    Debug.Log("Unable to place object, counter is full");
                }
            }
        }

        protected override void SettingsInteraction() {
            
        }

    }
}
