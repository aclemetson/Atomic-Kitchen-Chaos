using System;

namespace AtomicKitchenChaos.Messages
{
    [Serializable]
    public class CounterUnlockMessage : AtomicObjectRequestUnlockMessage
    {
        public string counterSOPath;

        public override void SubscriptionCheck(GameEventMessage payload) {
            CounterUnlockMessage temp = (CounterUnlockMessage)payload;
            if(temp.counterSOPath == counterSOPath) {
                unlockEvent.Invoke();
                unlockEvent.RemoveAllListeners();
                GameEventBus.Unsubscribe<CounterUnlockMessage>(SubscriptionCheck);
            }
        }
    }
}
