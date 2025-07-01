using System;
using UnityEngine.Events;

namespace AtomicKitchenChaos.Messages
{
    [Serializable]
    public class AtomicObjectRequestUnlockMessage : GameEventMessage
    {
        protected UnityEvent unlockEvent = new();

        public void AddUnlockAction(UnityAction action) {
            unlockEvent.AddListener(action);
        }

        public void TriggerEvent() {
            unlockEvent.Invoke();
        }
    }
}
