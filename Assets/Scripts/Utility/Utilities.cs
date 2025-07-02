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
        public static readonly int MAX_LEVEL_FILE_SIZE = 10 * 1024 * 1024;
        public static readonly string DIR_LEVEL_DATA = "Resources/Levels";
        public static readonly string DIR_LEVEL_REQUIREMENT_DATA = "Resources/LevelRequirements";
        public static readonly string DIR_GAME_OUTCOME_DATA = "Resources/GameOutcomes";

        public static string GetDataPath(string dataPath, string fileName="") {
            string fullPath = Path.Combine(Application.dataPath, dataPath);

            if (!Directory.Exists(fullPath)) {
                Directory.CreateDirectory(fullPath);
            }

            return Path.Combine(Application.dataPath, dataPath, fileName);
        }
        #endregion
    }
}
