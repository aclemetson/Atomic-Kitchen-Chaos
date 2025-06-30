using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Level;
using AtomicKitchenChaos.Utility;
using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor
{
    public class LevelCreatorWindow : EditorWindow
    {
        private string levelName = "NewLevel";
        private LevelRequirementSO levelRequirementSO;
        private Vector2 scrollPos;
        private List<CounterEntry> counterEntries = new();
        private int tileHeight = 100;

        [MenuItem("Tools/Level Tools/Level Creator", priority = 0)]
        public static void ShowFullscreenWindow() {
            var window = GetWindow<LevelCreatorWindow>();
            window.titleContent = new GUIContent("Level Creator");

            Rect mainDisplay = UnityEditorInternal.InternalEditorUtility.GetBoundsOfDesktopAtPoint(Vector2.zero);
            float margin = 40f;

            window.position = new Rect(
                mainDisplay.x + margin,
                mainDisplay.y + margin,
                mainDisplay.width - 2 * margin,
                mainDisplay.height - 4 * margin
            );

            window.Show();
        }

        [MenuItem("Tools/Level Tools/Edit Existing Level", priority = 1)]
        public static void EditExistingLevel() {
            LoadLevelFromDisk();
        }

        private void OnGUI() {

            if(counterEntries.Count == 0) {
                counterEntries.Add(default);
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
            GUILayout.Label("Level Information", EditorStyles.boldLabel);

            // Level Name Field
            levelName = EditorGUILayout.TextField("Level Name", levelName);

            // Recipt List SO
            EditorGUILayout.LabelField("Recipe List", EditorStyles.label);
            levelRequirementSO = (LevelRequirementSO)EditorGUILayout.ObjectField(levelRequirementSO, typeof(LevelRequirementSO), false);

            // Small space before grid header
            GUILayout.Space(10);
            GUILayout.Label("Counters", EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Begin Scrollview for Grid
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(tileHeight));

            // Counter SO Objects
            DrawCounterList();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button($"+ Add Counter")) {
                counterEntries.Add(default);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Save Level File")) {
                SaveLevelFile();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Load Another Level")) {
                LoadLevelFromDisk();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private void DrawCounterList(){
            int removeIndex = -1;
            int columns = Mathf.Max(1, Mathf.FloorToInt(position.width / 300f));
            int currentColumn = 0;

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < counterEntries.Count; i++) {
                if (currentColumn == 0) {
                    EditorGUILayout.BeginHorizontal(); // Start a new row
                }

                EditorGUILayout.BeginVertical("box", GUILayout.Width(280));
                CounterEntry counterEntry = counterEntries[i];

                counterEntry.position = EditorGUILayout.Vector3Field("Position", counterEntry.position);
                counterEntry.counterSO = (CounterSO)EditorGUILayout.ObjectField("Counter SO", counterEntry.counterSO, typeof(CounterSO), false);
                counterEntry.isActive = EditorGUILayout.Toggle("Is Acitve", counterEntry.isActive);

                EditorGUI.BeginDisabledGroup(counterEntry.isActive);
                counterEntry.purchasePrice = EditorGUILayout.IntField("Purchase Price", counterEntry.purchasePrice);
                EditorGUI.EndDisabledGroup();


                if (counterEntry.isActive) 
                    counterEntry.purchasePrice = 0;

                if(GUILayout.Button("Remove", GUILayout.Width(80))) {
                    removeIndex = i;
                }

                counterEntries[i] = counterEntry;
                GUILayout.EndVertical();

                currentColumn++;
                if(currentColumn >= columns || i == counterEntries.Count - 1) {
                    EditorGUILayout.EndHorizontal();
                    currentColumn = 0;
                }
            }
            EditorGUILayout.EndVertical();

            if (removeIndex >= 0) { counterEntries.RemoveAt(removeIndex); }

        }

        private void SaveLevelFile() {
            if (string.IsNullOrEmpty(levelName)) {
                Debug.LogWarning("Level name is required.");
                return;
            }

            var levelData = new LevelData {
                LevelName = levelName,
                levelRequirementSOPath = AssetDatabase.GetAssetPath(levelRequirementSO),
                Counters = counterEntries.Select(c => new CounterData {
                    position = new CompressableStructs.CompressableVector3(c.position),
                    counterSOpath = AssetDatabase.GetAssetPath(c.counterSO),
                    isActive = c.isActive,
                    purchasePrice = c.purchasePrice,
                }).ToArray()
            };

            // Serialize to JSON
            string json = JsonUtility.ToJson(levelData, true);
            byte[] raw = Encoding.UTF8.GetBytes(json);
            int maxCompressedSize = LZ4Codec.MaximumOutputSize(raw.Length);
            byte[] compressed = new byte[maxCompressedSize];

            int compressedLength = LZ4Codec.Encode(raw, 0, raw.Length, compressed, 0, compressed.Length);
            Array.Resize(ref compressed, compressedLength);

            // Make sure it is less than the MAX FILE SIZE before saving
            if (compressedLength > Utilities.MAX_LEVEL_FILE_SIZE) {
                EditorUtility.DisplayDialog(
                    "File Too Large",
                    $"Level file is too large ({compressedLength / (1024 * 1024f):F2} MB). Max allowed is {Utilities.MAX_LEVEL_FILE_SIZE / (1024 * 1024):F2} MB.",
                    "OK"
                );
                return;
            }

            // Save to Resources folder
            string fullPath = Utilities.GetDataPath(Utilities.DIR_LEVEL_DATA, levelName + ".lz4");

            // Check if file exists
            if (File.Exists(fullPath)) {
                bool overwrite = EditorUtility.DisplayDialog(
                    "File Already Exists",
                    $"A level file named '{levelName}' laready exists.\nDo you want to overwrite it?",
                    "Yes", "No"
                );

                if (!overwrite) return;
            }

            // Save file
            File.WriteAllBytes(fullPath, compressed);
            AssetDatabase.Refresh();

            Debug.Log($"Saved level to: {fullPath}");
            Close();
        }

        private static void LoadLevelFromDisk() {
            string path = EditorUtility.OpenFilePanel("Select Level File", Utilities.GetDataPath(Utilities.DIR_LEVEL_DATA), "lz4");

            if (string.IsNullOrEmpty(path)) return;

            try {
                LevelData levelData = LevelLoader.LoadLevel(path, isFullPath: true);
                LevelCreatorWindow window = GetWindow<LevelCreatorWindow>("Level Creator");
                window.PopulateLevelData(levelData);
            } catch (Exception ex) {
                Debug.LogError($"Failed to load and parse level file:\n{ex}");
            }
        }

        private void PopulateLevelData(LevelData data) {
            levelName = data.LevelName;

            levelRequirementSO = AssetDatabase.LoadAssetAtPath<LevelRequirementSO>(data.levelRequirementSOPath);
            counterEntries = data.Counters.Select(c => new CounterEntry {
                position = c.position.ToVector3(),
                counterSO = AssetDatabase.LoadAssetAtPath<CounterSO>(c.counterSOpath),
                isActive = c.isActive,
                purchasePrice = c.purchasePrice
            }).ToList();

            Repaint();
        }
    }

    [Serializable]
    public struct CounterEntry {
        public Vector3 position;
        public CounterSO counterSO;
        public bool isActive;
        public int purchasePrice;
    }
}
