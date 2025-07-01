using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Game;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using AtomicKitchenChaos.Utility;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
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
                int capturedIndex = i;
                var atomicObjectRequest = submissionCounterSO.levelRequirementData.atomicObjectRequests[i];
                foreach(var message in atomicObjectRequest.unlockMessages) {
                    AtomicObjectRequestUnlockMessage temp = (AtomicObjectRequestUnlockMessage)message;
                    GameEventBus.AssignGenericUnlockSubscription(temp, () => { UnlockAtomicObjectRequest(capturedIndex); });
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
            GameEventBus.Publish(new UpdateQuarkMessage() { changeInQuarks = reward });
        }

        private void SetRequest() {
            LevelRequirementData.AtomicObjectRequest[] atomicObjectRequests = submissionCounterSO.levelRequirementData.atomicObjectRequests.Where(x => !x.isLocked).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, atomicObjectRequests.Length);

            LevelRequirementData.AtomicObjectRequest randomSelection = atomicObjectRequests[randomIndex];
            DataHandler.TryLoadSO(randomSelection.atomicObjectSOPath, out request);
            quantity = 1;
            reward = UnityEngine.Random.Range((int)randomSelection.rewardMinimum, (int)randomSelection.rewardMaximum);

            ingredientContainerUI.SetData(request, quantity);
        }

        private void UnlockAtomicObjectRequest(int index) {
            submissionCounterSO.levelRequirementData.atomicObjectRequests[index].isLocked = false;
        }
    }
}
