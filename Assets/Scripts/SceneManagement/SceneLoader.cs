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
            GameEventBus.Subscribe<LoadSceneMessage>(LoadScene);
        }

        private static void LoadScene(LoadSceneMessage payload) {
            levelDataPath = string.IsNullOrEmpty(payload.levelDataPath) ? string.Empty : payload.levelDataPath;
            LoadScene(payload.sceneName);
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
