using AtomicKitchenChaos.Game;
using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Misc
{
    public class SubmissionCounter : Counter {

        [SerializeField] private IngredientContainerUI ingredientContainerUI;

        [SerializeField] private AtomicObjectSO request;
        [SerializeField] private int quantity;
        private int reward = 10;

        protected override void Start() {
            base.Start();
            ingredientContainerUI.SetData(request, quantity);
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
                }
            }
        }

        protected override void SettingsInteraction() {
            throw new System.NotImplementedException();
        }

        private void ClaimReward() {
            GameManager.Instance.AddToQuarkCount(reward);
        }

        public void SetRequest(AtomicObjectSO request, int quantity) {
            this.request = request;
            this.quantity = quantity;
        }
    }
}
