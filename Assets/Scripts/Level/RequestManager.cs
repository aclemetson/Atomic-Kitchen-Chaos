using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Messages;
using System.Linq;
using UnityEngine;
namespace AtomicKitchenChaos.Level {
    internal class RequestManager {

        LevelRequirementData _levelRequirementData;
        internal RequestManager(LevelRequirementData levelRequirementData) {
            _levelRequirementData = levelRequirementData;
            SetLevelRequirementSubscribers();
            GameEventBus.Subscribe<GetAtomicRequestMessage>(SetRequest);
        }

        private void SetLevelRequirementSubscribers() {
            for (int i = 0; i < _levelRequirementData.atomicObjectRequests.Length; i++) {
                int requestIndex = i;
                var atomicObjectRequest = _levelRequirementData.atomicObjectRequests[i];
                for (int j = 0; j < atomicObjectRequest.unlockMessages.Length; j++) {
                    int messageIndex = j;
                    var message = atomicObjectRequest.unlockMessages[messageIndex];
                    AtomicObjectRequestUnlockMessage temp = (AtomicObjectRequestUnlockMessage)message;
                    GameEventBus.AssignGenericUnlockSubscription(temp, () => { UnlockAtomicObjectRequest(requestIndex, messageIndex); });
                }
            }
        }

        private void UnlockAtomicObjectRequest(int requestIndex, int messageIndex) {
            UnlockMessage message = (UnlockMessage)_levelRequirementData.atomicObjectRequests[requestIndex].unlockMessages[messageIndex];
            message.Unlock();
            _levelRequirementData.atomicObjectRequests[requestIndex].unlockMessages[messageIndex] = message;

            bool[] unlockFlags = _levelRequirementData.atomicObjectRequests[requestIndex].unlockMessages.Select(x => ((UnlockMessage)x).IsLocked).ToArray();
            bool final = unlockFlags.Any(x => x);

            _levelRequirementData.atomicObjectRequests[requestIndex].isLocked = final;

            if (!final) {
                GameEventBus.Publish(new AtomicObjectRequestSuccessMessage() {
                    atomicObjectSOPath = _levelRequirementData.atomicObjectRequests[requestIndex].atomicObjectSOPath
                });
            }
        }

        private void SetRequest(GetAtomicRequestMessage payload) {
            LevelRequirementData.AtomicObjectRequest[] atomicObjectRequests = _levelRequirementData.atomicObjectRequests.Where(x => !x.isLocked).ToArray();
            int randomIndex = Random.Range(0, atomicObjectRequests.Length);

            LevelRequirementData.AtomicObjectRequest randomSelection = atomicObjectRequests[randomIndex];
            int reward = Random.Range((int)randomSelection.rewardMinimum, (int)randomSelection.rewardMaximum);
            int quantity = Random.Range(1, 4);

            SetAtomicRequestMessage message = new SetAtomicRequestMessage() {
                counterID = payload.counterID,
                atomicObjectSOPath = randomSelection.atomicObjectSOPath,
                quantity = quantity,
                reward = reward * quantity,
            };

            GameEventBus.Publish(message);
        }
    }
}
