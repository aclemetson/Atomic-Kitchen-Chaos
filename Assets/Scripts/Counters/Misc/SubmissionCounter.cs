using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Misc
{
    public class SubmissionCounter : Counter {

        [SerializeField] private IngredientContainerUI ingredientContainerUI;
        [SerializeField] private SubmissionCounterUI submissionCounterUI;

        private AtomicObjectSO request;
        private int quantity, reward;

        protected override void Start() {
            base.Start();
            GameEventBus.Subscribe<SetAtomicRequestMessage>(SetRequest);
            GameEventBus.Publish(new GetAtomicRequestMessage() { counterID = gameObject.GetInstanceID() });
        }

        protected override void Interact() {
            if(playerManager.HasAtomicObject() && request != null && quantity > 0) {
                if(playerManager.AtomicObject.AtomicObjectSO == request) {
                    quantity--;
                    playerManager.RemoveAtomicObject();
                    ingredientContainerUI.SetQuantity(quantity);
                }

                if(quantity == 0) {
                    ClaimReward();
                    GameEventBus.Publish(new GetAtomicRequestMessage() { counterID = gameObject.GetInstanceID() });
                }
            }
        }

        protected override void SettingsInteraction() {
            throw new System.NotImplementedException();
        }

        private void ClaimReward() {
            GameEventBus.Publish(new AddQuarks() { changeInQuarks = reward });
        }

        private void SetRequest(SetAtomicRequestMessage message) {
            if (message.counterID == gameObject.GetInstanceID()) {
                DataHandler.TryLoadSO(message.atomicObjectSOPath, out request);
                quantity = message.quantity;
                reward = message.reward;
                ingredientContainerUI.SetData(request, quantity);
                submissionCounterUI.SetReward(reward);
            }
        }
    }
}
