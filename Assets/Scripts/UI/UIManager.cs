using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using Codice.Client.Common.GameUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance;
        
        [SerializeField] private Canvas canvas;
        [SerializeField] private LevelSelectorPopulatorUI levelSelectorPopulatorUI;
        [SerializeField] private SettingsPopulatorUI settingsPopulatorUI;
        [SerializeField] private StoragePanelPopulatorUI storagePanelPopulatorUI;
        [SerializeField] private ExoticMaterialPanelUI exoticMaterialPanelUI;
        [SerializeField] private HUDUI hudUI;
        [SerializeField] private GameObject mainMenuUI;
        [SerializeField] private Image background;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                DontDestroyOnLoad(canvas);
                SetSceneUI(Utilities.MAIN_MENU_SCENE);
            } else {
                Destroy(gameObject);
            }
        }

        public void SetSceneUI(string sceneName) {
            if(sceneName == Utilities.GAME_SCENE) {
                hudUI.enabled = true;
                exoticMaterialPanelUI.enabled = true;
                mainMenuUI.SetActive(false);
                background.gameObject.SetActive(false);
            } else {
                levelSelectorPopulatorUI.enabled = false;
                settingsPopulatorUI.enabled = false;
                storagePanelPopulatorUI.enabled = false;
                exoticMaterialPanelUI.enabled = false;
                hudUI.enabled = false;
            }
        }

        public void PopulateLevelSelector(string[] levelNames, UnityAction<int> action) {
            levelSelectorPopulatorUI.PopulateLevelSelection(levelNames, action);
        }

        public void PopulateSettingsMenu(string settingsText, List<ISettingsObject> list, UnityAction<int> action) {
            settingsPopulatorUI.PopulateSettingsMenu(settingsText, list, action);
        }

        public void PopulateStorageMenu(StorageData[] storageData, UnityAction<int> action) {
            storagePanelPopulatorUI.PopulateStorageMenu(storageData, action);
        }

        public void AddExoticMaterialUI(string material, int count) {
            exoticMaterialPanelUI.AddExoticMaterialUI(material, count);
        }

        public void SetQuarkCount(long quarkCount) {
            hudUI.SetQuarkCount(quarkCount);
        }
    }
}