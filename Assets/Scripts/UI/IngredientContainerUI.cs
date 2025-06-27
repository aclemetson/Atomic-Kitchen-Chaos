using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI {
    internal class IngredientContainerUI : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI ingredientLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;

        internal void SetIngredient(string name, int quantity) {
            gameObject.SetActive(true);
            ingredientLabel.text = name;
            quantityLabel.text = quantity.ToString();
        }
    }
}