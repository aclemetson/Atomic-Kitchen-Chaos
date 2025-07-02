using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Editor.MessageMappers;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor {
    public class LevelRequirementsCreatorWindow : EditorWindow {
        private string levelRequirementName = "NewLevelRequirement";
        private string gameOutcomeDataPath = "";
        private LevelRequirementData levelRequirementData;
        private GameOutcomeData gameOutcomeData;
        private List<LevelRequirementData.AtomicObjectRequest> atomicObjectRequests;
        private List<AtomicObjectSO> atomicObjectSOs;
        private Vector2 scrollPos;
        private int tileHeight = 100;

        private static Type[] availableEventTypes;

        [MenuItem("Tools/Level Tools/Level Requirements Creator", priority = 20)]
        public static void ShowFullscreenWindow() {
            var window = GetWindow<LevelRequirementsCreatorWindow>();
            window.titleContent = new GUIContent("Level Requirements Creator");

            Rect mainDisplay = UnityEditorInternal.InternalEditorUtility.GetBoundsOfDesktopAtPoint(Vector2.zero);
            float margin = 40f;

            window.position = new Rect(
                mainDisplay.x + margin,
                mainDisplay.y + margin,
                mainDisplay.width - 2 * margin,
                mainDisplay.height - 4 * margin
            );

            availableEventTypes = MessageMapper.ATOMIC_OBJECT_REQUEST_UNLOCK_MAPPER.Keys.ToArray();
            window.Show();
        }

        [MenuItem("Tools/Level Tools/Edit Existing Level Requirements", priority = 21)]
        public static void EditExistingLevelRequirements() {
            LoadLevelRequirementsFromDisk();
        }

        private void OnGUI() {
            if (atomicObjectRequests == null) {
                atomicObjectRequests = new();
            }

            if (atomicObjectRequests.Count == 0) {
                atomicObjectRequests.Add(default);
            }

            if (atomicObjectSOs == null) {
                atomicObjectSOs = new();
                atomicObjectSOs.Add(null);
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
            GUILayout.Label("Level Requirements Information", EditorStyles.boldLabel);

            levelRequirementName = EditorGUILayout.TextField("Level Requirement Name", levelRequirementName);
            GUILayout.BeginHorizontal();

            string assetName = string.IsNullOrEmpty(levelRequirementData.gameOutcomePath) ? "None Selected" : Path.GetFileNameWithoutExtension(levelRequirementData.gameOutcomePath);
            EditorGUILayout.LabelField(assetName, GUILayout.MaxWidth(200));

            // Get Game Outcomes by file
            EditorGUILayout.LabelField("Game Outcome Data", EditorStyles.label);

            if (GUILayout.Button("Browse", GUILayout.Width(80))) {
                string selectedPath = EditorUtility.OpenFilePanel(
                    "Select Game Outcome Asset",
                    Utilities.DIR_GAME_OUTCOME_DATA,
                    "lz4"
                    );
                if (!string.IsNullOrEmpty(selectedPath)) {
                    // Convert full path to relative project path
                    gameOutcomeDataPath = Utilities.GetUnityRelativeAssetPath(selectedPath);
                    if (!DataHandler.TryLoadFromFile(gameOutcomeDataPath, out gameOutcomeData)) {
                        Debug.LogError("Selected file is not valid");
                    }
                    levelRequirementData.gameOutcomePath = gameOutcomeDataPath;
                }
            }

            GUILayout.EndHorizontal();

            // Small space before grid header
            GUILayout.Space(10);
            GUILayout.Label("Atomic Object Requests", EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Begin Scrollview for Grid
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(tileHeight));

            // Atomic Object Request Structs
            DrawAtomicObjectRequestList();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button($"+ Add Atomic Object Request")) {
                atomicObjectRequests.Add(default);
                atomicObjectSOs.Add(null);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Save Level File")) {
                SaveLevelFile();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Load Another Level")) {
                LoadLevelRequirementsFromDisk();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            levelRequirementData.atomicObjectRequests = atomicObjectRequests.ToArray();
        }
        private void DrawAtomicObjectRequestList() {
            int removeIndex = -1;
            int columns = Mathf.Max(1, Mathf.FloorToInt(position.width / 300f));
            int currentColumn = 0;

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < atomicObjectRequests.Count; i++) {
                if (currentColumn == 0) {
                    EditorGUILayout.BeginHorizontal();
                }

                EditorGUILayout.BeginVertical("box", GUILayout.Width(280));
                LevelRequirementData.AtomicObjectRequest atomicObjectRequest = atomicObjectRequests[i];

                AtomicObjectSO atomicObjectSO = atomicObjectSOs[i];
                if (!string.IsNullOrEmpty(atomicObjectRequest.atomicObjectSOPath) && atomicObjectSO == null) {
                    DataHandler.TryLoadSO(atomicObjectRequest.atomicObjectSOPath, out atomicObjectSO);
                }
                atomicObjectSO = (AtomicObjectSO)EditorGUILayout.ObjectField("Atomic Object SO", atomicObjectSO, typeof(AtomicObjectSO), false);
                atomicObjectSOs[i] = atomicObjectSO;
                string assetPath = AssetDatabase.GetAssetPath(atomicObjectSO);
                if (atomicObjectRequest.atomicObjectSOPath != assetPath) {
                    atomicObjectRequest.atomicObjectSOPath = assetPath;
                }

                atomicObjectRequest.rewardMinimum = EditorGUILayout.LongField("Reward Minimum", atomicObjectRequest.rewardMinimum);
                atomicObjectRequest.rewardMaximum = EditorGUILayout.LongField("Reward Maximum", atomicObjectRequest.rewardMaximum);
                atomicObjectRequest.isLocked = EditorGUILayout.Toggle("Is Locked", atomicObjectRequest.isLocked);

                List<MessageEditor> allEvents = new();
                if (atomicObjectRequest.unlockMessages != null) {
                    foreach (var message in atomicObjectRequest.unlockMessages) {
                        if (MessageMapper.ATOMIC_OBJECT_REQUEST_UNLOCK_MAPPER.TryGetValue(message.GetType(), out var editorType)) {
                            var editor = (MessageEditor)Activator.CreateInstance(editorType, message);
                            allEvents.Add(editor);
                        }
                    }
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Atomic Object Request Unlock Message", EditorStyles.boldLabel);
                int eventsRemoveIndex = -1;

                for (int j = 0; j < allEvents.Count; j++) {
                    var evt = allEvents[j];

                    if (evt == null) continue;

                    EditorGUILayout.BeginVertical("box");

                    evt.EditorDrawingFunction();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Remove Game Message", GUILayout.Width(150))) {
                        eventsRemoveIndex = j;
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                // If the Remove Message Button is pushed
                if (eventsRemoveIndex >= 0) { allEvents.RemoveAt(eventsRemoveIndex); }

                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                // If the Add Message Button is pushed
                GenericMenu menu = LoadGameEventMessages(allEvents, i);
                if (EditorGUILayout.DropdownButton(new GUIContent("Add Game Message"), FocusType.Keyboard)) {
                    menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                }


                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    removeIndex = i;
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                atomicObjectRequest.unlockMessages = allEvents.Select(t => t.Message).ToArray();
                atomicObjectRequests[i] = atomicObjectRequest;
                GUILayout.EndVertical();

                currentColumn++;
                if (currentColumn >= columns || i == atomicObjectRequests.Count - 1) {
                    EditorGUILayout.EndHorizontal();
                    currentColumn = 0;
                }
            }
            EditorGUILayout.EndVertical();

            if (removeIndex >= 0) { atomicObjectRequests.RemoveAt(removeIndex); }
        }

        private static void LoadLevelRequirementsFromDisk() {
            string path = EditorUtility.OpenFilePanel("Select Level Requirement File", Utilities.GetDataPath(Utilities.DIR_LEVEL_REQUIREMENT_DATA), "lz4");

            if (string.IsNullOrEmpty(path)) return;

            try {
                if (DataHandler.TryLoadFromFile(path, out LevelRequirementData temp)) {
                    LevelRequirementsCreatorWindow window = GetWindow<LevelRequirementsCreatorWindow>("Level Requirements Creator");
                    window.PopulateLevelRequirementData(Path.GetFileNameWithoutExtension(path), temp);
                }
            } catch (Exception ex) {
                Debug.LogError($"Failed to load and parse level file:\n{ex}");
            }
        }

        private void PopulateLevelRequirementData(string filename, LevelRequirementData levelRequirementData) {
            levelRequirementName = filename;
            this.levelRequirementData = levelRequirementData;
            atomicObjectRequests = levelRequirementData.atomicObjectRequests.ToList();
            atomicObjectSOs = new();
            gameOutcomeDataPath = levelRequirementData.gameOutcomePath;
            if(!string.IsNullOrEmpty(gameOutcomeDataPath)) 
                DataHandler.TryLoadFromFile(gameOutcomeDataPath, out gameOutcomeData);

            foreach (var atomicObjectRequest in atomicObjectRequests) {
                AtomicObjectSO atomicObjectSO = null;
                DataHandler.TryLoadSO(atomicObjectRequest.atomicObjectSOPath, out atomicObjectSO);
                atomicObjectSOs.Add(atomicObjectSO);
            }

            availableEventTypes = MessageMapper.ATOMIC_OBJECT_REQUEST_UNLOCK_MAPPER.Keys.ToArray();
            Repaint();
        }

        private void SaveLevelFile() {
            if (string.IsNullOrEmpty(levelRequirementName)) {
                Debug.LogWarning("Level Requirement Name is required.");
                return;
            }

            string fullPath = Utilities.GetUnityRelativeAssetPath(Utilities.GetDataPath(Utilities.DIR_LEVEL_REQUIREMENT_DATA, levelRequirementName + ".lz4"));
            levelRequirementData.levelRequirementName = Path.GetFileNameWithoutExtension(fullPath);
            levelRequirementData.gameOutcomePath = gameOutcomeDataPath;

            if (DataHandler.TrySaveToFile(levelRequirementData, fullPath))
                Close();
        }

        private GenericMenu LoadGameEventMessages(List<MessageEditor> allEvents, int atomicIndex) {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < availableEventTypes.Length; i++) {
                var eventType = availableEventTypes[i];
                var data = (atomicIndex, allEvents, eventType);
                menu.AddItem(new GUIContent(eventType.Name), false, AddNewGameEventMessage, data);
            }
            return menu;
        }

        private void AddNewGameEventMessage(object userData) {
            var (atomicIndex, allEvents, messageType) = ((int, List<MessageEditor>, Type))userData;

            if (MessageMapper.ATOMIC_OBJECT_REQUEST_UNLOCK_MAPPER.TryGetValue(messageType, out Type editorType)) {
                var gameEventMessage = (GameEventMessage)Activator.CreateInstance(messageType);
                var newEvent = (MessageEditor)Activator.CreateInstance(editorType, gameEventMessage);

                allEvents.Add(newEvent);
            } else {
                Debug.LogError($"Unable to get type in mapper {messageType.GetType()}.");
            }

            var request = atomicObjectRequests[atomicIndex];
            request.unlockMessages = allEvents.Select(t => t.Message).ToArray();
            atomicObjectRequests[atomicIndex] = request;

            Repaint();
        }
    }
}
