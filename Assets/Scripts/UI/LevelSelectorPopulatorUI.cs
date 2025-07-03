using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.UI {
    internal class LevelSelectorPopulatorUI : MonoBehaviour {

        [SerializeField] private LevelSelectorContainerUI containerPrefab;
        [SerializeField] private Transform gridContainer;

        internal void PopulateLevelSelection(string[] levelNames, UnityAction<int> action) {
            gameObject.SetActive(true);
            for (int i = 0; i < levelNames.Length; i++) {
                var go = Instantiate(containerPrefab, gridContainer);
                go.SetLevelName(levelNames[i]);
                int temp = i;
                go.AddButtonAction(() => {
                    action.Invoke(temp);
                    CloseMenu();
                });
            }
        }

        private void CloseMenu() {
            foreach (Transform child in gridContainer) {
                if (child != containerPrefab.transform) {
                    Destroy(child.gameObject);
                }
            }
            gameObject.SetActive(false);
        }
    }
}
