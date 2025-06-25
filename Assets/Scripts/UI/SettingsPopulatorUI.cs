using AtomicKitchenChaos.GeneratedObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI {
    public class SettingsPopulatorUI : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI settingsLabel;
        [SerializeField] private SelectableContainerUI containerPrefab;
        [SerializeField] private Transform gridContainer;

        private int selectedObject = 0;

        public void PopulateSettingsMenu(string settingsText, List<ISettingsObject> list) {
            settingsLabel.text = settingsText;

            for (int i = 0; i < list.Count; i++) {
                var go = Instantiate(containerPrefab, gridContainer);
                go.gameObject.SetActive(true);
                go.AddButtonAction(() => {
                    selectedObject = i;
                    foreach(Transform child in transform)
                        Destroy(child);
                    gameObject.SetActive(false);
                    });

                if(i == selectedObject || (selectedObject > list.Count && i == 0)) {
                    go.Selected();
                } else {
                    go.Unselected();
                }
            }
        }
    }
}