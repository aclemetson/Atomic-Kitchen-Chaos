using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI {
    internal class SelectableContainerUI : MonoBehaviour {
        [SerializeField] private GameObject selectedBackground;
        [SerializeField] private GameObject unselectedBackground;
        [SerializeField] private GameObject lockedBackground;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI unlockPriceLabel;
        [SerializeField] private TextMeshProUGUI quarksLabel;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button unlockButton;
        [SerializeField] private IngredientPopulatorUI ingredientPopulatorUI;

        private bool willBeActive = false;
        private long unlockPrice;
        private ISettingsObject settingsObject;

        private void Awake() {
            unlockButton.onClick.AddListener(TryUnlock);
        }

        internal void Selected() {
            selectedBackground.SetActive(true);
            unselectedBackground.SetActive(false);
        }

        internal void Unselected() {
            selectedBackground.SetActive(false);
            unselectedBackground.SetActive(true);
        }

        private void TryUnlock() {
            GameEventBus.Publish(new TryUnlockMessage() { unlockPrice = unlockPrice, callback = UnlockAction });
        }

        private void UnlockAction() {
            UnlockVisuals();
            settingsObject.UnlockObject();
        }

        internal void AddButtonAction(UnityAction action) {
            selectButton.onClick.AddListener(action);
        }

        internal void SetData(ISettingsObject obj, bool willBeActive=false) {
            if(!obj.IsLocked) {
                UnlockVisuals();
            }
            this.willBeActive = willBeActive;
            gameObject.SetActive(true);
            settingsObject = obj;
            titleLabel.text = obj.DisplayName;
            unlockPrice = obj.UnlockCost;
            unlockPriceLabel.text = NumberFormatter.FormatNumber(unlockPrice);
        }

        internal void PopulateIngredients(Dictionary<AtomicObjectSO, int> ingredients) {
            ingredientPopulatorUI.PopulateIngredients(ingredients);
        }

        private void UnlockVisuals() {
            lockedBackground.SetActive(false);
            selectButton.gameObject.SetActive(true);
            selectButton.interactable = true;
            unlockButton.gameObject.SetActive(false);
            unlockPriceLabel.gameObject.SetActive(false);
            quarksLabel.gameObject.SetActive(false);

            selectedBackground.SetActive(willBeActive);
            ingredientPopulatorUI.gameObject.SetActive(true);
        }
    }
}