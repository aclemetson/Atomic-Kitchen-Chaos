using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.UI;
using AtomicKitchenChaos.Utility;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace AtomicKitchenChaos.SceneManagement
{
    public static class SceneLoader
    {
        private static string levelDataPath;

        public static string LevelDataPath => levelDataPath;

        public static void Init() {
            GameEventBus.Subscribe<QuitGameMessage>(QuitGame);
            GameEventBus.Subscribe<LoadLevelMessage>(LoadLevel);
        }


        private static void LoadLevel(LoadLevelMessage message) {
            levelDataPath = message.levelDataPath;
            LoadScene(Utilities.GAME_SCENE);
        }

        public static void LoadScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
            UIManager.Instance.SetSceneUI(sceneName);
        }

        private static void QuitGame(QuitGameMessage payload) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
