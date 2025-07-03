using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    internal class LevelSelectorContainerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameLabel;
        [SerializeField] private Button selectButton;

        internal void AddButtonAction(UnityAction action) {
            selectButton.onClick.AddListener(action);
        }

        internal void SetLevelName(string levelName) {
            gameObject.SetActive(true);
            levelNameLabel.text = levelName;
        }
    }
}
