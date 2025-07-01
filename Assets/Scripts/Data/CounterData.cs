using System;

namespace AtomicKitchenChaos.Data
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
