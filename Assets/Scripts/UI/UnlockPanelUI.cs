using AtomicKitchenChaos.Utility;
using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    public class UnlockPanelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterTypeLabelUI;
        [SerializeField] private TextMeshProUGUI counterPriceLabelUI;

        public void SetUnlockPanel(string labelText, long price) {
            counterTypeLabelUI.text = labelText;
            counterPriceLabelUI.text = NumberFormatter.FormatNumber(price);
        }
    }
}
