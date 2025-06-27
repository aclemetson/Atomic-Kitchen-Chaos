using System.Collections.Generic;
using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    internal class IngredientPopulatorUI : MonoBehaviour
    {
        [SerializeField] private IngredientContainerUI ingredientContainerPrefab;

        internal void PopulateIngredients(Dictionary<string, int> ingredientList) {
            foreach (var item in ingredientList) {
                var go = Instantiate(ingredientContainerPrefab, transform);
                go.SetIngredient(item.Key, item.Value);
            }
        }
    }
}
