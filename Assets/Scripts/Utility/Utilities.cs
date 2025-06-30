using System.IO;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace AtomicKitchenChaos.Utility
{
    public static class Utilities
    {
        #region Level Data
        public static readonly int MAX_LEVEL_FILE_SIZE = 10 * 1024 * 1024;
        public static readonly string DIR_LEVEL_DATA = "Resources/Levels";

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
