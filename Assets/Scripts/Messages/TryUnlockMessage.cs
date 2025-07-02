using UnityEngine.Events;

namespace AtomicKitchenChaos.Messages
{
    public class TryUnlockMessage : GameEventMessage
    {
        public long unlockPrice;
        public UnityAction callback;
    }
}
