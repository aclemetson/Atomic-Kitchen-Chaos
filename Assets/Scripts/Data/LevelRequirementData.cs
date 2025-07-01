using AtomicKitchenChaos.Messages;
using System;

namespace AtomicKitchenChaos.Data
{
    public struct LevelRequirementData {
        public string levelRequirementName;
        public AtomicObjectRequest[] atomicObjectRequests;
        public long levelCompletionTask;

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
