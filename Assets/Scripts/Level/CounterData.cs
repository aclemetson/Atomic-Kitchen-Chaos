using AtomicKitchenChaos.Data;
using System;

namespace AtomicKitchenChaos.Level
{
    [Serializable]
    public struct CounterData
    {
        public CompressableStructs.CompressableVector3 position;
        public string counterSOpath;
        public bool isActive;
        public int purchasePrice;
    }
}
