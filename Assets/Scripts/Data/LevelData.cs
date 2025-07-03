
using System;

namespace AtomicKitchenChaos.Data
{
    [Serializable]
    public struct LevelData 
    {
        public string levelName;
        public int levelIndex;
        public CounterData[] Counters;
        public string levelRequirementPath;
    }
}
