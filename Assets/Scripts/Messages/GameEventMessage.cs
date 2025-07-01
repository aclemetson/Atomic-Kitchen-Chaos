using System;

namespace AtomicKitchenChaos.Messages {

    [Serializable]
    public class GameEventMessage {
        public string displayName;

        public virtual void SubscriptionCheck(GameEventMessage payload) {
            throw new NotImplementedException();
        }
    }

}