using AtomicKitchenChaos.GeneratedObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance;

        [SerializeField] private SettingsPopulatorUI settingsPopulatorUI;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public void PopulateSettingsMenu(string settingsText, List<ISettingsObject> list, UnityAction<int> action) {
            settingsPopulatorUI.PopulateSettingsMenu(settingsText, list, action);
        }
    }
}