using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.Messages
{
    public class UnlockMessage : GameEventMessage
    {
        protected bool isLocked;

        public bool IsLocked => isLocked;

        protected UnityEvent unlockEvent = new();

        public override void SubscriptionCheck(GameEventMessage payload) {
            Trigger();
        }

        public void AddUnlockAction(UnityAction action) {
            isLocked = true;
            unlockEvent.AddListener(action);
        }

        protected void Trigger() {
            unlockEvent.Invoke();
            unlockEvent.RemoveAllListeners();
            GameEventBus.Unsubscribe<CounterUnlockMessage>(SubscriptionCheck);
        }

        public void Unlock() {
            isLocked = false;
        }
    }
}
