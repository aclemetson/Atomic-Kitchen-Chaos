using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    internal class StoragePanelPopulatorUI : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private StorageItemContainerUI containerPrefab;
        [SerializeField] private Transform gridContainer;

        [Header("Buttons")]
        [SerializeField] private Button backButton;


        private void Awake() {
            backButton.onClick.AddListener(CloseMenu);
        }

        internal void PopulateStorageMenu(StorageData[] storageData, UnityAction<int> action) {
            gameObject.SetActive(true);

            for (int i = 0; i < storageData.Length; i++) {
                var go = Instantiate(containerPrefab, gridContainer);
                DataHandler.TryLoadSO(storageData[i].AtomicObjectSOPath, out AtomicObjectSO atomicObjectSO);
                go.SetContainer(atomicObjectSO, storageData[i].quantity);
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
