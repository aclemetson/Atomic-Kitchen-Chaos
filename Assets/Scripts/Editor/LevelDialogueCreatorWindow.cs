using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Editor.MessageMappers;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor
{
    public class LevelDialogueCreatorWindow : CreatorWindow<DialogueData>
    {
        private static readonly string WINDOW_NAME = "Dialogue Creator";

        private string dialogueName = "NewDialogue";
        private DialogueData dialogueData;
        private List<DialogueData.DialogueEntryData> dialogueEntries;
        private List<GameEventMessage> gameEvents;

        private static Type[] availableEventTypes;

        [MenuItem("Tools/Level Tools/Dialogue Creator", priority = 60)]
        public static void ShowFullscreenWindow() {
            getWindowCallback = () => GetWindow<LevelDialogueCreatorWindow>(WINDOW_NAME);
            availableEventTypes = MessageMapper.DIALOGUE_TRIGGER_MAPPER.Keys.ToArray();
            ShowFullscreenWindow(WINDOW_NAME);
        }

        [MenuItem("Tools/Level Tools/Edit Existing Dialogue", priority = 61)]
        public static void EditExistingDialgue() {
            getWindowCallback = () => GetWindow<LevelDialogueCreatorWindow>(WINDOW_NAME);
            EditExistingObject(Utilities.DIR_DIALOGUE_DATA, WINDOW_NAME);
        }

        private void OnGUI() {
            if (dialogueEntries == null) {
                dialogueEntries = new();
            }

            if (dialogueEntries.Count == 0) {
                dialogueEntries.Add(default);
            }

            if (gameEvents == null) {
                gameEvents = new();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
            GUILayout.Label("Dialogue Information", EditorStyles.boldLabel);

            dialogueName = EditorGUILayout.TextField("Dialogue Name", dialogueName);

            // Add the Triggering Message
            List<MessageEditor> messageEditors = new();

            foreach(var message in gameEvents) {
                if (MessageMapper.DIALOGUE_TRIGGER_MAPPER.TryGetValue(message.GetType(), out Type editorType)) {
                    var editor = (MessageEditor)Activator.CreateInstance(editorType, message);
                    messageEditors.Add(editor);
                }
            }

            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < availableEventTypes.Length; i++) {
                var eventType = availableEventTypes[i];
                menu.AddItem(new GUIContent(eventType.Name), false, () => {
                    if(MessageMapper.DIALOGUE_TRIGGER_MAPPER.TryGetValue(eventType, out Type editorType)) {
                        var message = (GameEventMessage)Activator.CreateInstance(eventType);
                        gameEvents.Add(message);
                    }
                });
            }
            if (EditorGUILayout.DropdownButton(new GUIContent("Add Game Message"), FocusType.Keyboard)) {
                menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
            }

            GUILayout.Space(20);

            int indexToRemove = -1;
            for (int i = 0; i < messageEditors.Count; i++) {
                var editor = messageEditors[i];
                var message = gameEvents[i];

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(message.GetType().Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", GUILayout.Width(60))) {
                    indexToRemove = i;
                }
                EditorGUILayout.EndHorizontal();
                editor.EditorDrawingFunction();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Begin Scrollview for Grid
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(tileHeight));

            DrawDialogueList();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button($"+ Add Dialogue Text")) {
                dialogueEntries.Add(default);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Save Dialogue File")) {
                SaveObject();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Load Another Dialogue")) {
                LoadObjectFromDisk(Utilities.DIR_DIALOGUE_DATA, WINDOW_NAME);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if(indexToRemove >= 0) gameEvents.RemoveAt(indexToRemove);

            dialogueData.triggeringMessages = gameEvents.ToArray();
            dialogueData.messagesHaveTriggered = new bool[gameEvents.Count];
        }

        private void DrawDialogueList() {
            int removeIndex = -1;
            int columns = Mathf.Max(1, Mathf.FloorToInt(position.width / 600f));
            int currentColumn = 0;

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < dialogueEntries.Count; i++) {
                if (currentColumn == 0) {
                    EditorGUILayout.BeginHorizontal();
                }

                EditorGUILayout.BeginVertical("box", GUILayout.Width(600));
                DialogueData.DialogueEntryData dialogueEntry = dialogueEntries[i];

                dialogueEntry.message = EditorGUILayout.TextField("Message", dialogueEntry.message);
                dialogueEntry.pauseGame = EditorGUILayout.Toggle("Pause Game", dialogueEntry.pauseGame);

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    removeIndex = i;
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                dialogueEntries[i] = dialogueEntry;
                GUILayout.EndVertical();

                currentColumn++;
                if (currentColumn >= columns || i == dialogueEntries.Count - 1) {
                    EditorGUILayout.EndHorizontal();
                    currentColumn = 0;
                }
            }
            EditorGUILayout.EndVertical();

            if (removeIndex >= 0) { dialogueEntries.RemoveAt(removeIndex); }
            dialogueData.dialogueEntries = dialogueEntries.ToArray();
        }

        protected override void PopulateEditorData(DialogueData dialogueData) {
            dialogueName = dialogueData.dialogueName;
            this.dialogueData = dialogueData;
            dialogueEntries = dialogueData.dialogueEntries.ToList();
            gameEvents = dialogueData.triggeringMessages.ToList();

            availableEventTypes = MessageMapper.DIALOGUE_TRIGGER_MAPPER.Keys.ToArray();
        }

        protected override void SaveObject() {
            if (string.IsNullOrEmpty(dialogueName)) {
                Debug.LogWarning("Dialogue Name is required.");
                return;
            }

            dialogueData.dialogueName = dialogueName;

            SaveObjectFile(Utilities.DIR_DIALOGUE_DATA, dialogueName, dialogueData);
        }
    }
}
