using System;
using System.IO;
using UnityEngine;

namespace AtomicKitchenChaos.Utility
{
    public static class Utilities
    {
        #region Path Utilities
        public static string GetUnityRelativeAssetPath(string fullPath) {
            fullPath = fullPath.Replace('\\', '/');
            string projectPath = Application.dataPath.Replace('\\', '/');

            if (!fullPath.StartsWith(projectPath))
                throw new ArgumentException("Path must be inside Assets folder.");

            return "Assets" + fullPath.Substring(projectPath.Length);
        }
        #endregion

        #region Level Data
        public const int MAX_LEVEL_FILE_SIZE = 10 * 1024 * 1024;
        public const string DIR_LEVEL_DATA = "Resources/Levels";
        public const string DIR_LEVEL_REQUIREMENT_DATA = "Resources/LevelRequirements";
        public const string DIR_GAME_OUTCOME_DATA = "Resources/GameOutcomes";
        public const string DIR_DIALOGUE_DATA = "Resources/Dialogues";
        public const string DIR_DIALOGUE_BUNDLE_DATA = "Resources/Dialogues/Bundles";

        public static string GetDataPath(string dataPath, string fileName="") {
            string fullPath = Path.Combine(Application.dataPath, dataPath);

            if (!Directory.Exists(fullPath)) {
                Directory.CreateDirectory(fullPath);
            }

            return Path.Combine(Application.dataPath, dataPath, fileName);
        }

        public static int GetNumLevels(string relativeDataPath) {
            return GetFullFilePaths(relativeDataPath).Length;
        }

        public static string[] GetFullFilePaths(string relativeDataPath) {
            string absolutePath = Path.Combine(Application.dataPath, relativeDataPath);
            string extension = "*.lz4";

            if (Directory.Exists(absolutePath)) {
                string[] files = Directory.GetFiles(absolutePath, extension, SearchOption.TopDirectoryOnly);
                return files;
            }

            return default;
        }
        #endregion

        #region Scene Utilies

        public const string MAIN_MENU_SCENE = "MainMenuScene";
        public const string GAME_SCENE = "GameScene";
        public const string RTS_SCENE = "RTSScene";

        #endregion
    }
}
