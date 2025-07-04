using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    internal class FinalSubmissionUI : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private StorageItemContainerUI containerPrefab;
        [SerializeField] private Transform gridContainer;

        [Header("Buttons")]
        [SerializeField] private Button backButton;

        private void Awake() {
            backButton.onClick.AddListener(CloseMenu);
        }

        internal void PopulateFinalSubmissionPanel(AtomicObjectSO[] atomicObjectSOs, int[] quantities) {
            gameObject.SetActive(true);
            if (atomicObjectSOs.Length != quantities.Length) {
                Debug.LogError("Mismatch between final submission object and quantity lists.");
                return;
            }

            for (int i = 0; i < atomicObjectSOs.Length; i++) {
                int staticIndex = i;
                var go = Instantiate(containerPrefab, gridContainer);
                go.SetContainer(atomicObjectSOs[staticIndex], quantities[staticIndex]);
                go.AddButtonAction(() => {
                    // Open an ingredient panel to assist
                    Debug.Log($"Open ingredient Panel for {atomicObjectSOs[staticIndex].displayName}.");
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
