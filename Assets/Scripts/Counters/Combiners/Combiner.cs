using AtomicKitchenChaos.GeneratedObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Combiners {
    public class Combiner : Counter{

        [SerializeField] private RecipeListSO recipeList;

        private RecipeSO currentRecipe;

        private Dictionary<string, int> recipeCounts;

        protected override void Interact() {
            if (state == State.Idle) {
                StartWork();
            } else if (state == State.Full) {
                TryCollect();
            }
        }

        protected override void FinishedWork() {
            if (currentRecipe.result != null && currentRecipe.result.atomicObjectPrefab != null) {
                state = State.Full;
                storedObject = Instantiate(currentRecipe.result.atomicObjectPrefab, holdPosition);
            }
            if (isInteracted) {
                AddInteraction();
            }
        }

        protected override void Start() {
            base.Start();
            currentRecipe = recipeList.recipeList[0];
            recipeCounts = currentRecipe.GetRecipeDictionary();

        }

        protected override void StartWork() {
            Debug.Log("Work has been attempted");
            if(playerManager.HasAtomicObject()) {
                Debug.Log("Work has started");
                string objectName = playerManager.AtomicObject.DisplayName;
                if (recipeCounts.ContainsKey(objectName) && recipeCounts[objectName] > 0) {
                    Debug.Log($"Deposited {objectName}");
                    recipeCounts[objectName]--;
                    playerManager.RemoveAtomicObject();

                    // Check to see if all counts are 0 and start making the product
                    if (recipeCounts.All(x => x.Value == 0)) {
                        state = State.Working;
                        progressBar.SetFillTime(currentRecipe.result.generateTime);
                    }
                } else {
                    Debug.Log($"Recipe does not need {objectName}");
                }
            } else {
                Debug.Log($"Player does not have an object.");
            }
        }

        protected override void SettingsInteraction() {
            throw new System.NotImplementedException();
        }
    }
}