using System.Linq;

namespace AtomicKitchenChaos.Messages
{
    public class CombinerCompleteObjectMessage : UnlockMessage
    {
        public string[] atomicObjectSOPaths;

        public override void SubscriptionCheck(GameEventMessage payload) {
            CombinerCompleteObjectMessage message = (CombinerCompleteObjectMessage)payload;
            foreach (var path in atomicObjectSOPaths) {
                if (!atomicObjectSOPaths.Contains(path)) {
                    return;
                }
            }
            Trigger();
        }
    }
}
