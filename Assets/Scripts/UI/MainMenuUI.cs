using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            settingsButton.onClick.AddListener(RTSGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void Start() {
            pressAnyKeyToContinuePanel.SetAdvanceScreen(() => pressAnyKeyToContinuePanel.gameObject.SetActive(false));
        }

        private void QuitGame() {
            GameEventBus.Publish(new QuitGameMessage());
        }

        private void RTSGame() {
            // Temporary send to RTS Scene
            GameEventBus.Publish(new LoadSceneMessage() { sceneName = Utilities.RTS_SCENE });
        }

        private void LevelSelect() {
            string[] fileNames = Utilities.GetFullFilePaths(Utilities.DIR_LEVEL_DATA);
            string[] fileNamesOnly = fileNames.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
            UIManager.Instance.PopulateLevelSelector(fileNamesOnly, (index) => {
                GameEventBus.Publish(new LoadSceneMessage() { sceneName = Utilities.GAME_SCENE, levelDataPath = fileNames[index] });
            });
        }

        private void StartGame() {
            GameEventBus.Publish(new LoadSceneMessage() { sceneName = Utilities.GAME_SCENE, levelDataPath = "Assets/Resources/Levels/Level1.lz4" });
        }
    }
}
