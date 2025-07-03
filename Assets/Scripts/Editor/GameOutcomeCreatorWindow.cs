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
    public class GameOutcomeCreatorWindow : CreatorWindow<GameOutcomeData> {

        private static readonly string WINDOW_NAME = "Game Outcomes Creator";

        private string gameOutcomeName = "NewGameOutcome";
        private GameOutcomeData gameOutcomeData;
        private List<GameOutcomeData.GameOutcome> gameOutcomes;
        private List<Type> messageTypes;

        private static Type[] availableEventTypes;

        [MenuItem("Tools/Level Tools/Game Outcomes Creator", priority = 40)]
        public static void ShowFullscreenWindow() {
            getWindowCallback = () => GetWindow<GameOutcomeCreatorWindow>(WINDOW_NAME);
            availableEventTypes = MessageMapper.GAME_OUTCOME_UNLOCK_MAPPER.Keys.ToArray();
            ShowFullscreenWindow(WINDOW_NAME);
        }

        [MenuItem("Tools/Level Tools/Edit Existing Game Outcomes", priority = 41)]
        public static void EditExistingGameOutcomes() {
            EditExistingObject(Utilities.DIR_GAME_OUTCOME_DATA, WINDOW_NAME);
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
                SaveObject();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Load Another Level")) {
                LoadObjectFromDisk(Utilities.DIR_GAME_OUTCOME_DATA, WINDOW_NAME);
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
            gameOutcomeData.gameOutcomes = gameOutcomes.ToArray();
        }

        protected override void SaveObject() {
            if (string.IsNullOrEmpty(gameOutcomeName)) {
                Debug.LogWarning("Game Outcome Name is required.");
                return;
            }

            gameOutcomeData.gameOutcomeName = gameOutcomeName;

            SaveObjectFile(Utilities.DIR_GAME_OUTCOME_DATA, gameOutcomeName, gameOutcomeData);
        }

        protected override void PopulateEditorData(GameOutcomeData gameOutcomeData) {
            gameOutcomeName = gameOutcomeData.gameOutcomeName;
            this.gameOutcomeData = gameOutcomeData;
            gameOutcomes = gameOutcomeData.gameOutcomes.ToList();
            messageTypes = gameOutcomes.Select(x => x.message.GetType()).ToList();

            availableEventTypes = MessageMapper.GAME_OUTCOME_UNLOCK_MAPPER.Keys.ToArray();
        }
    }
}
