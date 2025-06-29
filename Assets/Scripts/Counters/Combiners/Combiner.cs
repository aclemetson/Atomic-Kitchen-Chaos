using AtomicKitchenChaos.Game;
using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Combiners {
    public class Combiner : Worker {

        [SerializeField] private RecipeListSO recipeList;
        [SerializeField] private CombinerRecipePanelUI recipePanelUI;

        private int recipeIndex = 0;
        private RecipeSO currentRecipe;

        private Dictionary<AtomicObjectSO, int> recipeCounts;

        protected override void Interact() {
            if (state == State.Idle) {
                StartWork();
            } else if (state == State.Full) {
                if(TryCollect()) {
                    ResetRecipe();
                    state = State.Idle;
                }
            }
        }

        protected override void FinishedWork() {
            if (currentRecipe.results != null) {
                state = State.Full;
                for (int i = 0; i < currentRecipe.results.Length; i++) {
                    var result = currentRecipe.results[i];
                    if (atomPrefab != null) {
                        var t = holdPositions[i].transform;
                        var go = Instantiate(atomPrefab, t);
                        SetAtomicObject(go, result);
                        SetLabel(go.atomicObjectSO, holdPositions[i].atomLabelContainerUI);
                    }
                }
                ExoticMaterialManager.Instance.HandleExoticMatter(currentRecipe.GetExoticMaterialCounts());
            }
            if (isNextTo) {
                AddInteraction();
            }
        }

        protected override void Start() {
            base.Start();
            SetNewRecipe(recipeIndex);
            foreach (var item in holdPositions) {
                ClearLabel(item.atomLabelContainerUI);
            }
        }

        protected override void StartWork() {
            if (playerManager.HasAtomicObject()) {
                AtomicObjectSO playerObjectSO = playerManager.AtomicObject.atomicObjectSO;
                if (recipeCounts.ContainsKey(playerObjectSO) && recipeCounts[playerObjectSO] > 0) {
                    recipeCounts[playerObjectSO]--;
                    playerManager.RemoveAtomicObject();
                    recipePanelUI.SetQuantity(playerObjectSO, recipeCounts[playerObjectSO]);

                    // Check to see if all counts are 0 and start making the product
                    if (recipeCounts.All(x => x.Value == 0)) {
                        state = State.Working;
                        progressBar.SetFillTime(currentRecipe.cookTime);
                        RemoveInteraction();
                    }
                } else {
                }
            }
        }

        protected override void SettingsInteraction() {
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
            ResetRecipe();

            if(currentRecipe.results.Length > holdPositions.Length) {
                Debug.LogError($"Recipe {currentRecipe.DisplayName} has more results than hold positions for this combiner. Remove from recipe list.");
            }
        }

        private void ResetRecipe() {
            recipeCounts = currentRecipe.GetRecipeDictionary();
            recipePanelUI.SetRecipePanelUI(currentRecipe.results[0], currentRecipe.materials);
        }
    }
}