using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using System.Linq;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Misc
{
    public class SubmissionCounter : Counter {

        [SerializeField] private IngredientContainerUI ingredientContainerUI;

        private int reward = 10;
        private SubmissionCounterSO submissionCounterSO;
        private AtomicObjectSO request;
        private int quantity;

        protected override void Start() {
            base.Start();
            submissionCounterSO = (SubmissionCounterSO)counterSO;
            SetRequest();
            for(int i = 0; i < submissionCounterSO.levelRequirementData.atomicObjectRequests.Length; i++) {
                int requestIndex = i;
                var atomicObjectRequest = submissionCounterSO.levelRequirementData.atomicObjectRequests[i];
                for(int j = 0; j < atomicObjectRequest.unlockMessages.Length; j++) {
                    int messageIndex = j;
                    var message = atomicObjectRequest.unlockMessages[messageIndex];
                    AtomicObjectRequestUnlockMessage temp = (AtomicObjectRequestUnlockMessage)message;
                    GameEventBus.AssignGenericUnlockSubscription(temp, () => { UnlockAtomicObjectRequest(requestIndex, messageIndex); });
                }
            }
        }

        protected override void Interact() {
            if(playerManager.HasAtomicObject() && request != null && quantity > 0) {
                if(playerManager.AtomicObject.atomicObjectSO == request) {
                    quantity--;
                    playerManager.RemoveAtomicObject();
                    ingredientContainerUI.SetQuantity(quantity);
                }

                if(quantity == 0) {
                    ClaimReward();
                    SetRequest();
                }
            }
        }

        protected override void SettingsInteraction() {
            throw new System.NotImplementedException();
        }

        private void ClaimReward() {
            GameEventBus.Publish(new AddQuarks() { changeInQuarks = reward });
        }

        private void SetRequest() {
            LevelRequirementData.AtomicObjectRequest[] atomicObjectRequests = submissionCounterSO.levelRequirementData.atomicObjectRequests.Where(x => !x.isLocked).ToArray();
            int randomIndex = Random.Range(0, atomicObjectRequests.Length);

            LevelRequirementData.AtomicObjectRequest randomSelection = atomicObjectRequests[randomIndex];
            DataHandler.TryLoadSO(randomSelection.atomicObjectSOPath, out request);
            quantity = 1;
            reward = Random.Range((int)randomSelection.rewardMinimum, (int)randomSelection.rewardMaximum);

            ingredientContainerUI.SetData(request, quantity);
        }

        private void UnlockAtomicObjectRequest(int requestIndex, int messageIndex) {
            UnlockMessage message = (UnlockMessage)submissionCounterSO.levelRequirementData.atomicObjectRequests[requestIndex].unlockMessages[messageIndex];
            message.Unlock();
            submissionCounterSO.levelRequirementData.atomicObjectRequests[requestIndex].unlockMessages[messageIndex] = message;

            bool[] unlockFlags = submissionCounterSO.levelRequirementData.atomicObjectRequests[requestIndex].unlockMessages.Select(x => ((UnlockMessage)x).IsLocked).ToArray();
            bool final = unlockFlags.Any(x => x);

            submissionCounterSO.levelRequirementData.atomicObjectRequests[requestIndex].isLocked = final;

            if(!final) {
                GameEventBus.Publish(new AtomicObjectRequestSuccessMessage() {
                    atomicObjectSOPath = submissionCounterSO.levelRequirementData.atomicObjectRequests[requestIndex].atomicObjectSOPath
                });
            }
        }
    }
}
