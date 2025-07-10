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
        [SerializeField] private Button selectButton;

        internal void SetCounterInformation(BuildData buildData) {
            gameObject.SetActive(true);
            nameLabelUI.text = buildData.name;
            selectButton.onClick.AddListener(buildData.selectAction);
        }

        internal void AddSelectButtonAction(UnityAction action) {
            selectButton.onClick.AddListener(action);
        }
    }
}
