using AtomicKitchenChaos.GeneratedObjects;
using TMPro;
using UnityEngine;

namespace AtomicKitchenChaos.UI
{
    public class AtomLabelContainerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI elementLabel;
        [SerializeField] private TextMeshProUGUI isotopeLabel;
        [SerializeField] private TextMeshProUGUI ionLabel;

        public void SetAtomPanel(AtomicObjectSO atomicObjectSO) {
            gameObject.SetActive(true);
            elementLabel.text = atomicObjectSO.atomicSymbol;
            isotopeLabel.text = atomicObjectSO.Nucleons.ToString();
            ionLabel.text = atomicObjectSO.Ionization;

            if (atomicObjectSO.atomicSymbol.Length == 1) {
                isotopeLabel.alignment = TextAlignmentOptions.BottomRight;
            } else if (atomicObjectSO.atomicSymbol.Length == 2) {
                isotopeLabel.alignment = TextAlignmentOptions.Bottom;
            }
        }

        public void ClearLabel() {
            gameObject.SetActive(false);
        }
    }
}
