using UnityEngine;
using UnityEngine.Events;
using BuildData = AtomicKitchenChaos.Messages.BuildDataChangeMessage.BuildData;

namespace AtomicKitchenChaos.UI
{
    internal class BuildMenuUI : MonoBehaviour
    {
        [SerializeField] private CounterContainerUI counterContainerPrefab;
        [SerializeField] private Transform gridContainer;

        private BuildData[] buildData;

        internal void SelectCounter(GameObject counterPrefab, UnityAction setSelectedAction) {
            setSelectedAction.Invoke();
        }

        internal void SetBuildMenuInformation(BuildData[] buildData) {
            this.buildData = buildData;
        }

        private void OnEnable() {
            // Destroy previous list
            foreach (Transform child in gridContainer) {
                if (child != counterContainerPrefab.transform) {
                    Destroy(child.gameObject);
                }
            }

            // Populate Previous List
            foreach (BuildData bd in buildData) {
                var go = Instantiate(counterContainerPrefab, gridContainer);
                go.SetCounterInformation(bd);
                go.AddSelectButtonAction(() => CloseMenu());
            }
        }

        private void CloseMenu() {
            gameObject.SetActive(false);
        }
    }
}
