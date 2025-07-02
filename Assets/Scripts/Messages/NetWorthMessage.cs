using System;
using System.Diagnostics;
using UnityEditor;

namespace AtomicKitchenChaos.Messages
{
    [Serializable]
    public class NetWorthMessage : AtomicObjectRequestUnlockMessage
    {
        public long value;

        public override void SubscriptionCheck(GameEventMessage payload) {
            NetWorthMessage temp = (NetWorthMessage)payload;
            if (temp.value >= value) {
                Trigger();
            }
        }
    }
}
