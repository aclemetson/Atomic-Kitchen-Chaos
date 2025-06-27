using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.Players;
using AtomicKitchenChaos.UI;
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


        [SerializeField] protected Transform[] holdPositions;
        [SerializeField] protected ProgressBarUI progressBar;


        protected State state = State.Idle;
        protected AtomicObject storedObject;


        protected override void Start() {
            base.Start();
            progressBar.OnTimeElapsed.AddListener(FinishedWork);
        }

        protected abstract void StartWork();
        protected abstract void FinishedWork();



        protected bool TryCollect() {
            if (!playerManager.HasAtomicObject()) {
                playerManager.SetAtomicObject(storedObject);
                storedObject = null;
                return true;
            } 
            return false;
        }
    }
}
