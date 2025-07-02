using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Editor.MessageMappers;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor {
    public class GameOutcomeCreatorWindow : EditorWindow {

        private string gameOutcomeName = "NewGameOutcome";
        private GameOutcomeData gameOutcomeData;
        private List<GameOutcomeData.GameOutcome> gameOutcomes;
        private List<Type> messageTypes;
        private Vector2 scrollPos;
        private int tileHeight = 100;

        private static Type[] availableEventTypes;

        [MenuItem("Tools/Level Tools/Game Outcomes Creator", priority = 40)]
        public static void ShowFullscreenWindow() {
            var window = GetWindow<GameOutcomeCreatorWindow>();
            window.titleContent = new GUIContent("Game Outcomes Creator");

            Rect mainDisplay = UnityEditorInternal.InternalEditorUtility.GetBoundsOfDesktopAtPoint(Vector2.zero);
            float margin = 40f;

            window.position = new Rect(
                mainDisplay.x + margin,
                mainDisplay.y + margin,
                mainDisplay.width - 2 * margin,
                mainDisplay.height - 4 * margin
            );

            availableEventTypes = MessageMapper.GAME_OUTCOME_UNLOCK_MAPPER.Keys.ToArray();
            window.Show();
        }

        [MenuItem("Tools/Level Tools/Edit Existing Game Outcomes", priority = 41)]
        public static void EditExistingGameOutcomes() {
            LoadGameOutcomesFromDisk();
        }

        private void OnGUI() {
            if (gameOutcomes == null) {
                gameOutcomes = new();
            }

            if (gameOutcomes.Count == 0) {
                gameOutcomes.Add(default);
            }

            if (messageTypes == null) {
                messageTypes = new();
                messageTypes.Add(null);
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
            GUILayout.Label("Game Outcomes Information", EditorStyles.boldLabel);

            gameOutcomeName = EditorGUILayout.TextField("Game Outcome Name", gameOutcomeName);

            // Small space before grid header
            GUILayout.Space(10);
            GUILayout.Label("Game Outcomes", EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Begin Scrollview for Grid
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(tileHeight));

            DrawGameOutcomeList();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button($"+ Add Atomic Object Request")) {
                gameOutcomes.Add(default);
                messageTypes.Add(null);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Save Level File")) {
                SaveGameOutcomeFile();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Load Another Level")) {
                LoadGameOutcomesFromDisk();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private void DrawGameOutcomeList() {
            int removeIndex = -1;
            int columns = Mathf.Max(1, Mathf.FloorToInt(position.width / 300f));
            int currentColumn = 0;

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < gameOutcomes.Count; i++) {
                int staticIndex = i;
                if (currentColumn == 0) {
                    EditorGUILayout.BeginHorizontal();
                }

                EditorGUILayout.BeginVertical("box", GUILayout.Width(280));
                GameOutcomeData.GameOutcome gameOutcome = gameOutcomes[i];

                gameOutcome.status = (GameOutcomeData.GameOutcomeStatus)EditorGUILayout.EnumPopup("Game Outcome Status", gameOutcome.status);

                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                Type editorType = null;
                if (messageTypes[staticIndex] != null) {
                    MessageMapper.GAME_OUTCOME_UNLOCK_MAPPER.TryGetValue(messageTypes[staticIndex], out editorType);
                }

                // If the Add Message Button is pushed
                GenericMenu menu = new GenericMenu();
                for (int j = 0; j < availableEventTypes.Length; j++) {
                    var eventType = availableEventTypes[j];
                    menu.AddItem(new GUIContent(eventType.Name), false, () => {
                        messageTypes[staticIndex] = eventType;
                        gameOutcome.message = (GameEventMessage)Activator.CreateInstance(eventType);
                        gameOutcomes[staticIndex] = gameOutcome;
                    });
                }
                if (EditorGUILayout.DropdownButton(new GUIContent("Add Game Message"), FocusType.Keyboard)) {
                    menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                }

                if (gameOutcome.message != null) {
                    MessageMapper.GAME_OUTCOME_UNLOCK_MAPPER.TryGetValue(gameOutcome.message.GetType(), out editorType);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (editorType != null) {
                    var editor = (MessageEditor)Activator.CreateInstance(editorType, gameOutcome.message);
                    editor.EditorDrawingFunction();
                }

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    removeIndex = i;
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                gameOutcomes[i] = gameOutcome;
                messageTypes[i] = editorType;
                GUILayout.EndVertical();

                currentColumn++;
                if (currentColumn >= columns || i == gameOutcomes.Count - 1) {
                    EditorGUILayout.EndHorizontal();
                    currentColumn = 0;
                }
            }
            EditorGUILayout.EndVertical();

            if (removeIndex >= 0) { gameOutcomes.RemoveAt(removeIndex); }
            gameOutcomeData.gameOutcomes = gameOutcomes.ToArray(); ;
        }

        private void SaveGameOutcomeFile() {
            if (string.IsNullOrEmpty(gameOutcomeName)) {
                Debug.LogWarning("Game Outcome Name is required.");
                return;
            }

            string fullPath = Utilities.GetUnityRelativeAssetPath(Utilities.GetDataPath(Utilities.DIR_GAME_OUTCOME_DATA, gameOutcomeName + ".lz4"));
            gameOutcomeData.gameOutcomeName = Path.GetFileNameWithoutExtension(fullPath);

            if (DataHandler.TrySaveToFile(gameOutcomeData, fullPath))
                Close();
        }

        private static void LoadGameOutcomesFromDisk() {
            string path = EditorUtility.OpenFilePanel("Select Game Outcome File", Utilities.GetDataPath(Utilities.DIR_GAME_OUTCOME_DATA), "lz4");

            if (string.IsNullOrEmpty(path)) return;

            try {
                if(DataHandler.TryLoadFromFile(path, out GameOutcomeData temp)) {
                    GameOutcomeCreatorWindow window = GetWindow<GameOutcomeCreatorWindow>("Game Outcomes Creator");
                    window.PopulateGameOutcomesData(Path.GetFileNameWithoutExtension(path), temp);
                }
            } catch (Exception ex) {
                Debug.LogError($"Failed to load and parse game outcomes file:\n{ex}");
            }
        }

        private void PopulateGameOutcomesData(string name, GameOutcomeData gameOutcomeData) {
            gameOutcomeName = name;
            this.gameOutcomeData = gameOutcomeData;
            gameOutcomes = gameOutcomeData.gameOutcomes.ToList();
            messageTypes = gameOutcomes.Select(x => x.message.GetType()).ToList();

            availableEventTypes = MessageMapper.GAME_OUTCOME_UNLOCK_MAPPER.Keys.ToArray();
        }
    }
}
