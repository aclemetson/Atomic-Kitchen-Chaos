using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor {
    public class LevelCreatorWindow : CreatorWindow<LevelData> {
        private static readonly string WINDOW_NAME = "Level Creator";

        private string levelName = "NewLevel";
        private LevelRequirementData levelRequirementData;
        private string levelRequirementDataPath = "";
        private DialogueBundleData dialogueBundleData;
        private string dialogueBundlePath = "";
        private List<CounterEntry> counterEntries = new();

        private static int levelIndex;

        [MenuItem("Tools/Level Tools/Level Creator", priority = 0)]
        public static void ShowFullscreenWindow() {
            levelIndex = Utilities.GetNumLevels(Utilities.DIR_LEVEL_DATA);
            getWindowCallback = () => GetWindow<LevelCreatorWindow>(WINDOW_NAME);
            ShowFullscreenWindow(WINDOW_NAME);
        }

        [MenuItem("Tools/Level Tools/Edit Existing Level", priority = 1)]
        public static void EditExistingLevel() {
            getWindowCallback = () => GetWindow<LevelCreatorWindow>(WINDOW_NAME);
            EditExistingObject(Utilities.DIR_LEVEL_DATA, WINDOW_NAME);
        }

        private void OnGUI() {

            if (counterEntries.Count == 0) {
                counterEntries.Add(default);
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(600));
            GUILayout.Label("Level Information", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            // Level Name Field
            levelName = EditorGUILayout.TextField("Level Name", levelName);
            levelIndex = EditorGUILayout.IntField("Level Index", levelIndex);
            GUILayout.EndHorizontal();

            // Level Requirement Data
            EditorGUILayout.LabelField("Level Requirement Data", EditorStyles.label);

            GUILayout.BeginHorizontal();

            string levelRequirementsAsset = string.IsNullOrEmpty(levelRequirementData.levelRequirementName) ? "None Selected" : levelRequirementData.levelRequirementName;
            EditorGUILayout.LabelField(levelRequirementsAsset, GUILayout.MaxWidth(200));

            if (GUILayout.Button("Browse", GUILayout.Width(80))) {
                string selectedPath = EditorUtility.OpenFilePanel(
                    "Select Level Requirement Asset",
                    Utilities.DIR_LEVEL_REQUIREMENT_DATA,
                    "lz4"
                    );
                if (!string.IsNullOrEmpty(selectedPath)) {
                    // Convert full path to relative project path
                    levelRequirementDataPath = Utilities.GetUnityRelativeAssetPath(selectedPath);
                    if (!DataHandler.TryLoadFromFile(levelRequirementDataPath, out levelRequirementData)) {
                        Debug.LogError("Selected file is not valid");
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            // Dialogue Bundle Data
            EditorGUILayout.LabelField("Dialogue Bundle Data", EditorStyles.label);

            GUILayout.BeginHorizontal();

            string dialogueBundleAsset = string.IsNullOrEmpty(dialogueBundlePath) ? "None Selected" : Path.GetFileNameWithoutExtension(dialogueBundlePath);
            EditorGUILayout.LabelField(dialogueBundleAsset, GUILayout.MaxWidth(200));

            if (GUILayout.Button("Browse", GUILayout.Width(80))) {
                string selectedPath = EditorUtility.OpenFilePanel(
                    "Select Dialogue Bundle Asset",
                    Utilities.DIR_DIALOGUE_BUNDLE_DATA,
                    "lz4"
                    );
                if (!string.IsNullOrEmpty(selectedPath)) {
                    // Conver full path to relative project path
                    dialogueBundlePath = Utilities.GetUnityRelativeAssetPath(selectedPath);
                    if(!DataHandler.TryLoadFromFile(dialogueBundlePath, out dialogueBundleData)) {
                        Debug.LogError("Selected file is not valid");
                    }
                }
            }

            GUILayout.EndHorizontal();

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
                SaveObject();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Load Another Level")) {
                LoadObjectFromDisk(Utilities.DIR_LEVEL_DATA, WINDOW_NAME);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private void DrawCounterList() {
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

                if (counterEntry.counterSO != null && counterEntry.counterSO.GetType() == typeof(CombinerSO)) {
                    var combinerSO = (CombinerSO)counterEntry.counterSO;
                    combinerSO.recipeListSO = (RecipeListSO)EditorGUILayout.ObjectField("Recipe List SO", combinerSO.recipeListSO, typeof(RecipeListSO), false);
                    counterEntry.counterSO = combinerSO;
                }

                counterEntry.isActive = EditorGUILayout.Toggle("Is Acitve", counterEntry.isActive);

                EditorGUI.BeginDisabledGroup(counterEntry.isActive);
                counterEntry.purchasePrice = EditorGUILayout.IntField("Purchase Price", counterEntry.purchasePrice);
                EditorGUI.EndDisabledGroup();


                if (counterEntry.isActive)
                    counterEntry.purchasePrice = 0;

                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    removeIndex = i;
                }

                counterEntries[i] = counterEntry;
                GUILayout.EndVertical();

                currentColumn++;
                if (currentColumn >= columns || i == counterEntries.Count - 1) {
                    EditorGUILayout.EndHorizontal();
                    currentColumn = 0;
                }
            }
            EditorGUILayout.EndVertical();

            if (removeIndex >= 0) { counterEntries.RemoveAt(removeIndex); }

        }

        protected override void SaveObject() {
            if (string.IsNullOrEmpty(levelName)) {
                Debug.LogWarning("Level name is required.");
                return;
            }

            var levelData = new LevelData {
                levelName = levelName,
                levelIndex = levelIndex,
                levelRequirementPath = levelRequirementDataPath,
                dialogueBundlePath = dialogueBundlePath,
                Counters = counterEntries.Select(c => new CounterData {
                    position = new CompressableStructs.CompressableVector3(c.position),
                    counterSOpath = AssetDatabase.GetAssetPath(c.counterSO),
                    isActive = c.isActive,
                    purchasePrice = c.purchasePrice,
                }).ToArray()
            };

            SaveObjectFile(Utilities.DIR_LEVEL_DATA, levelName, levelData);
        }

        protected override void PopulateEditorData(LevelData data) {
            levelName = data.levelName;

            if (!DataHandler.TryLoadFromFile(data.levelRequirementPath, out levelRequirementData)) {
                Debug.LogError("Unable to load Level Requirement Data");
            }

            levelRequirementDataPath = data.levelRequirementPath;
            dialogueBundlePath = data.dialogueBundlePath;

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
