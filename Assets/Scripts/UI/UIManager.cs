using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BuildData = AtomicKitchenChaos.Messages.BuildDataChangeMessage.BuildData;

namespace AtomicKitchenChaos.UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance;

        #region GameScene
        [Header("GameScene")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private LevelSelectorPopulatorUI levelSelectorPopulatorUI;
        [SerializeField] private CombinerSettingsPopulatorUI combinerSettingsPopulatorUI;
        [SerializeField] private StoragePanelPopulatorUI storagePanelPopulatorUI;
        [SerializeField] private FinalSubmissionUI finalSubmissionUI;
        [SerializeField] private ExoticMaterialPanelUI exoticMaterialPanelUI;
        [SerializeField] private DialoguePanelUI dialoguePanelUI;
        [SerializeField] private GameOverPanelUI gameOverPanelUI;
        [SerializeField] private HUDUI hudUI;
        [SerializeField] private MainMenuUI mainMenuUI;
        [SerializeField] private Image background;

        private bool menuIsUp = false;

        private bool hasStartedUp = false;

        public bool MenuIsUp => menuIsUp;

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

        private void Start() {
            dialoguePanelUI.SetCloseMenu(() => {
                GameEventBus.Publish(new DialogueHasFinishedMessage() { dialogueName = dialoguePanelUI.DialogueData.dialogueName });
            });
            GameEventBus.Subscribe<BuildDataChangeMessage>(SetBuildMenuInformation);
        }

        public void SetSceneUI(string sceneName) {
            switch(sceneName) {
                case Utilities.GAME_SCENE:
                    mainMenuUI.gameObject.SetActive(false);
                    hudUI.gameObject.SetActive(true);
                    exoticMaterialPanelUI.gameObject.SetActive(true);
                    break;

                case Utilities.MAIN_MENU_SCENE:

                    if (!hasStartedUp) {
                        mainMenuUI.gameObject.SetActive(true);
                        hasStartedUp = true;
                    }

                    break;

                case Utilities.RTS_SCENE:
                    mainMenuUI.gameObject.SetActive(false);
                    hudUI.gameObject.SetActive(true);
                    break;

                default:
                    hudUI.gameObject.SetActive(false);
                    exoticMaterialPanelUI.gameObject.SetActive(false);
                    break;
            }

            // All Scenes
            buildMenuUI.gameObject.SetActive(false);
            levelSelectorPopulatorUI.gameObject.SetActive(false);
            storagePanelPopulatorUI.gameObject.SetActive(false);
            finalSubmissionUI.gameObject.SetActive(false);
            gameOverPanelUI.gameObject.SetActive(false);
        }

        public void PopulateLevelSelector(string[] levelNames, UnityAction<int> action) {
            menuIsUp = true;
            levelSelectorPopulatorUI.PopulateLevelSelection(levelNames, action);
        }

        public void PopulateSettingsMenu(string settingsText, List<ISettingsObject> list, UnityAction<int> action) {
            menuIsUp = true;
            combinerSettingsPopulatorUI.PopulateSettingsMenu(settingsText, list, action);
        }

        public void PopulateStorageMenu(StorageData[] storageData, UnityAction<int> action) {
            menuIsUp = true;
            storagePanelPopulatorUI.PopulateStorageMenu(storageData, action);
        }

        public void PopulateFinalSubmissionPanel(AtomicObjectSO[] atomicObjectSOs, int[] quantities) {
            menuIsUp = true;
            finalSubmissionUI.PopulateFinalSubmissionPanel(atomicObjectSOs, quantities);
        }

        public void AddExoticMaterialUI(string material, int count) {
            exoticMaterialPanelUI.AddExoticMaterialUI(material, count);
        }

        public void SetQuarkCount(long quarkCount) {
            hudUI.SetQuarkCount(quarkCount);
        }

        public void StartDialogue(DialogueData dialogueData) {
            menuIsUp = true;
            dialoguePanelUI.StartDialogue(dialogueData);
        }
        
        public void SetMainMenuLoadingAction(UnityAction mainMenuLoadingAction) {
            gameOverPanelUI.SetMainMenuLoadingAction(mainMenuLoadingAction);
        }

        public void LevelFinished() {
            menuIsUp = true;
            gameOverPanelUI.LevelFinished();
        }

        #endregion

        #region RTSScene

        [Header("RTSScene")]
        [SerializeField] private BuildMenuUI buildMenuUI;

        private void SetBuildMenuInformation(BuildDataChangeMessage payload) {
            buildMenuUI.SetBuildMenuInformation(payload.buildData);
        }

        #endregion
    }
}