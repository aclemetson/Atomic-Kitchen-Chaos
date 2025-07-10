using UnityEngine;

namespace AtomicKitchenChaos.Messages
{
    public class AtomicFinalSubmissionRequestMessage : UnlockMessage
    {
        public string atomicObjectSOPath;
        public int quantity;

        public override void SubscriptionCheck(GameEventMessage payload) {
            AtomicFinalSubmissionRequestMessage message = (AtomicFinalSubmissionRequestMessage)payload;
            if(message.atomicObjectSOPath == atomicObjectSOPath && message.quantity <= 0) {
                Unlock();
                Trigger();
            }
        }
    }
}
