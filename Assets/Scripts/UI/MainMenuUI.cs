using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtomicKitchenChaos.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private PressAnyKeyToContinueUI pressAnyKeyToContinuePanel;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button levelSelectButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Awake() {
            startGameButton.onClick.AddListener(StartGame);
            levelSelectButton.onClick.AddListener(LevelSelect);
            settingsButton.onClick.AddListener(Settings);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void Start() {
            pressAnyKeyToContinuePanel.SetAdvanceScreen(() => pressAnyKeyToContinuePanel.gameObject.SetActive(false));
        }

        private void QuitGame() {
            GameEventBus.Publish(new QuitGameMessage());
        }

        private void Settings() {
            throw new NotImplementedException();
        }

        private void LevelSelect() {
            string[] fileNames = Utilities.GetFullFilePaths(Utilities.DIR_LEVEL_DATA);
            string[] fileNamesOnly = fileNames.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
            UIManager.Instance.PopulateLevelSelector(fileNamesOnly, (index) => {
                GameEventBus.Publish(new LoadLevelMessage() { levelDataPath = fileNames[index] });
            });
        }

        private void StartGame() {
            GameEventBus.Publish(new LoadLevelMessage() { levelDataPath = "Assets/Resources/Levels/NewLevel.lz4" });
        }
    }
}
