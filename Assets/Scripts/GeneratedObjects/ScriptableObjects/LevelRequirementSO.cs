using AtomicKitchenChaos.Messages;
using System;
using UnityEngine;

namespace AtomicKitchenChaos.GeneratedObjects.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Levels/LevelRequirementSO")]
    public class LevelRequirementSO : ScriptableObject
    {
        [SerializeField] public AtomicObjectRequest[] atomicObjectRequests;
        [SerializeField] public long levelCompletionTask;

        public struct AtomicObjectRequest {
            public AtomicObjectSO atomicObjectSO;
            public long rewardMinimum;
            public long rewardMaximum;
            public bool isLocked;
            public AtomicObjectRequestUnlockMessage unlockMessage;
        }
    }
}
