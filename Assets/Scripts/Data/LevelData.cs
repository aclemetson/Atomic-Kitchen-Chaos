
using System;

namespace AtomicKitchenChaos.Data
{
    [Serializable]
    public struct LevelData 
    {
        public string LevelName;
        public CounterData[] Counters;
        public string levelRequirementPath;
    }
}
