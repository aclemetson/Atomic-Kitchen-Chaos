using AtomicKitchenChaos.Game;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.UI;
using System.Linq;
using UnityEditor.PackageManager.Requests;
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
            GameManager.Instance.AddToQuarkCount(reward);
        }

        private void SetRequest() {
            LevelRequirementSO.AtomicObjectRequest[] atomicObjectRequests = submissionCounterSO.levelRequirementSO.atomicObjectRequests.Where(x => !x.isLocked).ToArray();
            int randomIndex = Random.Range(0, atomicObjectRequests.Length);

            LevelRequirementSO.AtomicObjectRequest randomSelection = atomicObjectRequests[randomIndex];
            request = randomSelection.atomicObjectSO;
            quantity = 1;
            reward = Random.Range((int)randomSelection.rewardMinimum, (int)randomSelection.rewardMaximum);

            ingredientContainerUI.SetData(request, quantity);
        }
    }
}
