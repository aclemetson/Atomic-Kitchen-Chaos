using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.GeneratedObjects.AtomicObjects;
using AtomicKitchenChaos.UI;
using System.Linq;
using UnityEngine;

namespace AtomicKitchenChaos.Counters
{
    public abstract class Worker : Counter
    {
        protected enum State {
            Idle,
            Working,
            Full,
        }

        [SerializeField] protected HoldPosition[] holdPositions;
        [SerializeField] protected ProgressBarUI progressBar;
        [SerializeField] protected AtomicObject atomPrefab;

        protected State state = State.Idle;

        protected override void Start() {
            base.Start();
            progressBar.OnTimeElapsed.AddListener(FinishedWork);
        }

        protected abstract void StartWork();
        protected abstract void FinishedWork();



        protected bool TryCollect() {
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
                            atomLabelContainerUI = item.atomLabelContainerUI;
                            closest = atomic;
                        }
                    }
                }

                if (closest != null) {
                    // There is an object to grab
                    playerManager.SetAtomicObject(closest);
                    ClearLabel(atomLabelContainerUI);
                    return holdPositions.All(t => t.transform.GetComponentInChildren<AtomicObject>() == null);
                }
            }
            return false;
        }
    }
}
