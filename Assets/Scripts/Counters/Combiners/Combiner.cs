using AtomicKitchenChaos.Game;
using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Counters.Combiners {
    public class Combiner : Worker {

        [SerializeField] private CombinerRecipePanelUI recipePanelUI;

        private CombinerSO combinerSO;
        private RecipeListSO recipeListSO;
        private int recipeIndex = 0;
        private RecipeSO currentRecipe;

        private Dictionary<AtomicObjectSO, int> recipeCounts;
        protected override void Start() {
            base.Start();

            combinerSO = (CombinerSO)counterSO;
            recipeListSO = combinerSO.recipeListSO;

            SetNewRecipe(recipeIndex);
            foreach (var item in holdPositions) {
                ClearLabel(item.atomLabelContainerUI);
            }
        }

        protected override void Interact() {
            if (state == State.Idle && currentRecipe != null) {
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
                        go.SetAtomicObjectSO(result);
                        SetLabel(result, holdPositions[i].atomLabelContainerUI);
                    }
                }
                GameEventBus.Publish(new CombinerCompleteObjectMessage() { atomicObjectSOPaths = currentRecipe.results.Select(t => AssetDatabase.GetAssetPath(t)).ToArray() });
                ExoticMaterialManager.Instance.HandleExoticMatter(currentRecipe.GetExoticMaterialCounts());
            }
            if (isNextTo) {
                AddInteraction();
            }
        }

        protected override void StartWork() {
            if (playerManager.HasAtomicObject()) {
                AtomicObjectSO playerObjectSO = playerManager.AtomicObject.AtomicObjectSO;
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
            UIManager.Instance.PopulateSettingsMenu("Select Recipe", recipeListSO.recipeList.Cast<ISettingsObject>().ToList(), SetNewRecipe);
        }

        private void SetNewRecipe(int recipeIndex) {

            if(recipeListSO == null || recipeListSO.recipeList.All(t => t.IsLocked)) {
                return;
            }

            if (recipeListSO.recipeList.Count == 0) {
                Debug.LogError($"Recipe List {recipeListSO.displayName} does not have any recipes.");
                return;
            }

            if (recipeIndex >= recipeListSO.recipeList.Count) {
                Debug.LogError($"Recipe List {recipeListSO.displayName} has an inconsistent indexing issue.");
                return;
            }

            currentRecipe = recipeListSO.recipeList[recipeIndex];
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