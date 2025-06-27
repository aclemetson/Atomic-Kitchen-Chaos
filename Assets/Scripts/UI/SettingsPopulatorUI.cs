using AtomicKitchenChaos.GeneratedObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.UI {
    internal class SettingsPopulatorUI : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI settingsLabel;
        [SerializeField] private SelectableContainerUI containerPrefab;
        [SerializeField] private Transform gridContainer;

        private int selectedObject = 0;
        private UnityEvent<int> selectedEvent;

        private void Awake() {
            selectedEvent = new UnityEvent<int>();
        }

        internal void PopulateSettingsMenu(string settingsText, List<ISettingsObject> list, UnityAction<int> action) {
            settingsLabel.text = settingsText;
            gameObject.SetActive(true);
            selectedEvent.AddListener(action);

            for (int i = 0; i < list.Count; i++) {
                var go = Instantiate(containerPrefab, gridContainer);
                go.SetLabel(list[i].DisplayName);
                int temp = i;
                go.AddButtonAction(() => {
                    selectedObject = temp;
                    selectedEvent.Invoke(selectedObject);
                    selectedEvent.RemoveListener(action);
                    foreach (Transform child in gridContainer) {
                        if (child != containerPrefab.transform) {
                            Destroy(child.gameObject);
                        }
                    }
                    gameObject.SetActive(false);
                    });

                // If these are recipes, populate the ingredients
                if (list[i] is RecipeSO recipe) {
                    SetIngredients(go, recipe);
                }

                if(i == selectedObject || (selectedObject > list.Count && i == 0)) {
                    go.Selected();
                } else {
                    go.Unselected();
                }
            }
        }

        private void SetIngredients(SelectableContainerUI obj, RecipeSO recipe) {
            Dictionary<string, int> ingredients = new();
            foreach (var material in recipe.materials) {
                ingredients.Add(material.atomicObject.DisplayName, material.quantity);
            }
            obj.PopulateIngredients(ingredients);
        }
    }
}