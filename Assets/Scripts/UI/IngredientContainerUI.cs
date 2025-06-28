using AtomicKitchenChaos.GeneratedObjects;
using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI {
    public class IngredientContainerUI : MonoBehaviour {

        [SerializeField] private AtomLabelContainerUI ingredientLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;

        public void SetData(AtomicObjectSO atomicObjectSO, int quantity) {
            gameObject.SetActive(true);
            ingredientLabel.SetAtomPanel(atomicObjectSO);
            SetQuantity(quantity);
        }

        public void SetQuantity(int quantity) {
            quantityLabel.text = $"x{quantity}";
        }
    }
}