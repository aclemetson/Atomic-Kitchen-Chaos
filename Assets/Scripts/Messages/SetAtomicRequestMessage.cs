using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.Messages
{
    public class SetAtomicRequestMessage : GameEventMessage
    {
        public int counterID;
        public string atomicObjectSOPath;
        public int quantity;
        public int reward;
    }
}
