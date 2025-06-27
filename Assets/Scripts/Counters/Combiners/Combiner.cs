using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Combiners {
    public class Combiner : Worker {

        [SerializeField] private RecipeListSO recipeList;

        private int recipeIndex = 0;
        private RecipeSO currentRecipe;

        private Dictionary<string, int> recipeCounts;

        protected override void Interact() {
            Debug.Log($"Combiner Interaction: {state}");
            if (state == State.Idle) {
                StartWork();
            } else if (state == State.Full) {
                if(TryCollect()) {
                    recipeCounts = currentRecipe.GetRecipeDictionary();
                    state = State.Idle;
                }
            }
        }

        protected override void FinishedWork() {
            if (currentRecipe.results != null) {
                state = State.Full;
                for (int i = 0; i < currentRecipe.results.Length; i++) {
                    var result = currentRecipe.results[i];
                    if (result.atomicObjectPrefab != null) {
                        var t = holdPositions[i];
                        storedObject = Instantiate(result.atomicObjectPrefab, t);
                        storedObject.DisplayName = result.displayName;
                    }
                }
            }
            if (isNextTo) {
                AddInteraction();
            }
        }

        protected override void Start() {
            base.Start();
            SetNewRecipe(recipeIndex);
        }

        protected override void StartWork() {
            Debug.Log("Work has been attempted");
            if (playerManager.HasAtomicObject()) {
                Debug.Log("Work has started");
                string objectName = playerManager.AtomicObject.DisplayName;
                if (recipeCounts.ContainsKey(objectName) && recipeCounts[objectName] > 0) {
                    Debug.Log($"Deposited {objectName}");
                    recipeCounts[objectName]--;
                    playerManager.RemoveAtomicObject();

                    // Check to see if all counts are 0 and start making the product
                    if (recipeCounts.All(x => x.Value == 0)) {
                        state = State.Working;
                        progressBar.SetFillTime(currentRecipe.cookTime);
                        RemoveInteraction();
                    }
                } else {
                    Debug.Log($"Recipe does not need {objectName}");
                }
            } else {
                Debug.Log($"Player does not have an object.");
            }
        }

        protected override void SettingsInteraction() {
            Debug.Log($"Settings Interaction");
            UIManager.Instance.PopulateSettingsMenu("Select Recipe", recipeList.recipeList.Cast<ISettingsObject>().ToList(), SetNewRecipe);
        }

        private void SetNewRecipe(int recipeIndex) {
            if (recipeList.recipeList.Count == 0) {
                Debug.LogError($"Recipe List {recipeList.displayName} does not have any recipes.");
                return;
            }

            if (recipeIndex >= recipeList.recipeList.Count) {
                Debug.LogError($"Recipe List {recipeList.displayName} has an inconsistent indexing issue.");
                return;
            }

            currentRecipe = recipeList.recipeList[recipeIndex];
            recipeCounts = currentRecipe.GetRecipeDictionary();

            if(currentRecipe.results.Length > holdPositions.Length) {
                Debug.LogError($"Recipe {currentRecipe.DisplayName} has more results than hold positions for this combiner. Remove from recipe list.");
            }
        }
    }
}