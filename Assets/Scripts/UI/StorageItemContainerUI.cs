using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    public class StorageItemContainerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI atomicObjectLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;
        [SerializeField] private Button selectButton;

        internal void SetContainer(string name, int quantity) {
            gameObject.SetActive(true);
            atomicObjectLabel.text = name;
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
