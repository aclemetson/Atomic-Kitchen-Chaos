using AtomicKitchenChaos.Messages;
using System;

namespace AtomicKitchenChaos.Data
{
    [Serializable]
    public struct LevelRequirementData {
        public string levelRequirementName;
        public AtomicObjectRequest[] atomicObjectRequests;
        public string gameOutcomePath;

        [Serializable]
        public struct AtomicObjectRequest {
            public string atomicObjectSOPath;
            public long rewardMinimum;
            public long rewardMaximum;
            public bool isLocked;
            public GameEventMessage[] unlockMessages;
        }
    }
}
