using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    public class StorageItemContainerUI : MonoBehaviour
    {
        [SerializeField] private AtomLabelContainerUI atomicObjectLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;
        [SerializeField] private Button selectButton;

        internal void SetContainer(AtomicObjectSO atomicObjectSO, int quantity) {
            gameObject.SetActive(true);
            atomicObjectLabel.SetAtomPanel(atomicObjectSO);
            if (quantity > 1) {
                quantityLabel.text = $"x{quantity}";
            } else {
                quantityLabel.text = "";
            }
        }

        internal void AddButtonAction(UnityAction unityAction) {
            selectButton.onClick.AddListener(unityAction);
        }

    }
}
