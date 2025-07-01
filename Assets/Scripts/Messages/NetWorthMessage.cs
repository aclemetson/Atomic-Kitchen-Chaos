using System;
using UnityEditor;

namespace AtomicKitchenChaos.Messages
{
    [Serializable]
    public class NetWorthMessage : AtomicObjectRequestUnlockMessage
    {
        public long value;
    }
}
