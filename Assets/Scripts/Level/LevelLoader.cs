using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Utility;
using K4os.Compression.LZ4;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace AtomicKitchenChaos.Level
{
    public static class LevelLoader
    {
        public static LevelData LoadLevel(string levelName, bool isFullPath=false) {
            string fullPath = isFullPath ? levelName : Utilities.GetDataPath(Utilities.DIR_LEVEL_DATA, levelName + ".lz4");

            DataHandler.TryLoadFromFile(fullPath, out LevelData levelData);
            return levelData;
        }
    }
}
