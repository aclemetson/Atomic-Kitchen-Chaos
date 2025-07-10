using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.Messages
{
    public class GameOverMessage : UnlockMessage
    {

        public override void SubscriptionCheck(GameEventMessage payload) {
            Trigger();
        }
    }
}
