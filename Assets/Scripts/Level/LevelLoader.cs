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

            if(!File.Exists(fullPath)) {
                Debug.LogError($"Level file not found at: {fullPath}");
                return default;
            }

            try {
                byte[] compressed = File.ReadAllBytes(fullPath);

                // Estimate raw size and allocate buffer
                byte[] decompressed = new byte[Utilities.MAX_LEVEL_FILE_SIZE];
                int decodedLength = LZ4Codec.Decode(compressed, 0, compressed.Length, decompressed, 0, decompressed.Length);
                string json = Encoding.UTF8.GetString(decompressed, 0, decodedLength);

                LevelData levelData = JsonUtility.FromJson<LevelData>(json);
                Debug.Log($"Loaded Level: {levelData.LevelName}");
                return levelData;
            } catch (Exception ex) {
                Debug.LogError($"Failed to load or decompress level: {ex}");
            }
            return default;
        }
    }
}
