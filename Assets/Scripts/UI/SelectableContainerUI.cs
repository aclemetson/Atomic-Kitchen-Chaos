using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI {
    public class SelectableContainerUI : MonoBehaviour {
        [SerializeField] private GameObject selectedBackground;
        [SerializeField] private GameObject unselectedBackground;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private Button selectButton;
        [SerializeField] private IngredientPopulatorUI ingredientPopulatorUI;

        internal void Selected() {
            selectedBackground.SetActive(true);
            unselectedBackground.SetActive(false);
        }

        internal void Unselected() {
            selectedBackground.SetActive(false);
            unselectedBackground.SetActive(true);
        }

        internal void AddButtonAction(UnityAction action) {
            selectButton.onClick.AddListener(action);
        }

        internal void SetLabel(string label) {
            gameObject.SetActive(true);
            titleLabel.text = label;
        }

        internal void PopulateIngredients(Dictionary<string, int> ingredients) {
            ingredientPopulatorUI.PopulateIngredients(ingredients);
        }
    }
}