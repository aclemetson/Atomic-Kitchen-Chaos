using AtomicKitchenChaos.GeneratedObjects;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    public class CombinerRecipePanelUI : MonoBehaviour
    {
        [SerializeField] private AtomLabelContainerUI recipeTextLabel;
        [SerializeField] private IngredientContainerUI ingredientContainerUIprefab;
        [SerializeField] private Transform ingredientList;

        private Dictionary<AtomicObjectSO, IngredientContainerUI> ingredientContainers;

        public void SetRecipePanelUI(AtomicObjectSO recipeResult, RecipeSO.MaterialCount[] materials) {
            ingredientContainers = new();
            recipeTextLabel.SetAtomPanel(recipeResult);

            // Remove all children from ingredient list
            foreach(Transform t in ingredientList) {
                if(t != ingredientContainerUIprefab.transform) {
                    Destroy(t.gameObject);
                }
            }

            foreach (var item in materials) {
                var go = Instantiate(ingredientContainerUIprefab, ingredientList);
                go.SetData(item.atomicObject, item.quantity);
                ingredientContainers[item.atomicObject] = go;
            }
        }

        public void SetQuantity(AtomicObjectSO ingredient, int quantity) {
            var container = ingredientContainers.FirstOrDefault(x => x.Key == ingredient);
            if(container.Value != null) {
                container.Value.SetQuantity(quantity);
            }
        }
    }
}
