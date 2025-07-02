
namespace AtomicKitchenChaos.Messages
{
    public class AtomicObjectRequestSuccessMessage : AtomicObjectRequestUnlockMessage
    {
        public string atomicObjectSOPath;

        public override void SubscriptionCheck(GameEventMessage payload) {
            AtomicObjectRequestSuccessMessage temp = (AtomicObjectRequestSuccessMessage)payload;
            if(temp.atomicObjectSOPath == atomicObjectSOPath) {
                Trigger();
            }
        }
    }
}
