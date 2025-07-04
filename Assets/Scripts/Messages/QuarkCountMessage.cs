using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.Messages
{
    public class QuarkCountMessage : UnlockMessage
    {
        public long quarkCount;

        public override void SubscriptionCheck(GameEventMessage payload) {
            QuarkCountMessage message = (QuarkCountMessage)payload;
            if(message.quarkCount >= quarkCount) {
                Trigger();
            }
        }
    }
}
