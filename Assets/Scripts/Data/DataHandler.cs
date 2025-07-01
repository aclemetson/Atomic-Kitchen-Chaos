using K4os.Compression.LZ4;
using System.Text;
using System;
using UnityEngine;
using UnityEditor;
using AtomicKitchenChaos.Utility;
using System.IO;
using Newtonsoft.Json;

namespace AtomicKitchenChaos.Data
{
    public static class DataHandler
    {
        private static readonly JsonSerializerSettings SERIALIZING_SETTINGS = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public static bool TrySaveToFile(object objData, string filePath) {
            // Serialize to JSON
            int compressedLength = CalculateFileSize(objData, out byte[] compressed);

            // Make sure it is less than the MAX FILE SIZE before saving
            if (compressedLength > Utilities.MAX_LEVEL_FILE_SIZE) {
                EditorUtility.DisplayDialog(
                    "File Too Large",
                    $"Level file is too large ({compressedLength / (1024 * 1024f):F2} MB). Max allowed is {Utilities.MAX_LEVEL_FILE_SIZE / (1024 * 1024):F2} MB.",
                    "OK"
                );
                return false;
            }

#if UNITY_EDITOR
            // Check if file exists for Unity Editor, overite otherwise
            if (File.Exists(filePath)) {
                bool overwrite = EditorUtility.DisplayDialog(
                    "File Already Exists",
                    $"A level file named '{Path.GetFileName(filePath)}' already exists.\nDo you want to overwrite it?",
                    "Yes", "No"
                );

                if (!overwrite) return false;
            }
#endif

            // Save file
            File.WriteAllBytes(filePath, compressed);
            AssetDatabase.Refresh();

            Debug.Log($"Saved level to: {filePath}");

            return true;
        }

        public static bool TrySaveSO(ScriptableObject scriptableObject, string filePath) {

#if UNITY_EDITOR
            // Check if file exists for Unity Editor, overite otherwise
            if (File.Exists(filePath)) {
                bool overwrite = EditorUtility.DisplayDialog(
                    "File Already Exists",
                    $"A file named '{Path.GetFileName(filePath)}' already exists.\nDo you want to overwrite it?",
                    "Yes", "No"
                );

                if (!overwrite) return false;
            }
#endif

            // Save file
            AssetDatabase.CreateAsset(scriptableObject, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Saved file to: {filePath}");

            return true;
        }

        public static bool TryLoadFromFile<T>(string filePath, out T objData) {
            if (!File.Exists(filePath)) {
                Debug.LogError($"File not found at: {filePath}");
                objData = default;
                return false;
            }

            try {
                byte[] compressed = File.ReadAllBytes(filePath);

                // Estimate raw size and allocate buffer
                byte[] decompressed = new byte[Utilities.MAX_LEVEL_FILE_SIZE];
                int decodedLength = LZ4Codec.Decode(compressed, 0, compressed.Length, decompressed, 0, decompressed.Length);
                string json = Encoding.UTF8.GetString(decompressed, 0, decodedLength);

                objData = JsonConvert.DeserializeObject<T>(json, SERIALIZING_SETTINGS);
                return true;
            } catch (Exception ex) {
                Debug.LogError($"Failed to load or decompress file: {ex}");
            }

            objData = default;
            return false;
        }

        public static bool TryLoadSO<T>(string filePath, out T scriptableObject) where T : ScriptableObject {
            if (!File.Exists(filePath)) {
                Debug.LogError($"Scriptable Object not found at: {filePath}");
                scriptableObject = default;
                return false;
            }

            try {
                scriptableObject = AssetDatabase.LoadAssetAtPath<T>(filePath);
                return true;
            } catch (Exception ex) {
                Debug.LogError($"Failed to load Scriptable Object: {ex}");
            }

            scriptableObject = default;
            return false;
        }

        private static int CalculateFileSize(object objData, out byte[] compressedData) {
            // Serialize to JSON
            string json = JsonConvert.SerializeObject(objData, SERIALIZING_SETTINGS);
            byte[] raw = Encoding.UTF8.GetBytes(json);
            int maxCompressedSize = LZ4Codec.MaximumOutputSize(raw.Length);
            compressedData = new byte[maxCompressedSize];

            int compressedLength = LZ4Codec.Encode(raw, 0, raw.Length, compressedData, 0, compressedData.Length);
            Array.Resize(ref compressedData, compressedLength);
            return compressedLength;
        }
    }
}
