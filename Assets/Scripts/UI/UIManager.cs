using AtomicKitchenChaos.GeneratedObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AtomicKitchenChaos.UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance;

        [SerializeField] private SettingsPopulatorUI settingsPopulatorUI;
        [SerializeField] private ExoticMaterialPanelUI exoticMaterialPanelUI;
        [SerializeField] private HUDUI hudUI;

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

        public void AddExoticMaterialUI(string material, int count) {
            exoticMaterialPanelUI.AddExoticMaterialUI(material, count);
        }

        public void SetQuarkCount(long quarkCount) {
            hudUI.SetQuarkCount(quarkCount);
        }
    }
}