using AtomicKitchenChaos.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BuildData = AtomicKitchenChaos.Messages.BuildDataChangeMessage.BuildData;

namespace AtomicKitchenChaos.UI
{
    internal class CounterContainerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabelUI;
        [SerializeField] private TextMeshProUGUI priceLabelUI;
        [SerializeField] private Button selectButton;

        internal void SetCounterInformation(BuildData buildData) {
            gameObject.SetActive(true);
            nameLabelUI.text = buildData.name;
            priceLabelUI.text = NumberFormatter.FormatNumber(buildData.price);
            selectButton.onClick.AddListener(buildData.selectAction);
        }

        internal void AddSelectButtonAction(UnityAction action) {
            selectButton.onClick.AddListener(action);
        }
    }
}
