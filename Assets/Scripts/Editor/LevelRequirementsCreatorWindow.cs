using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using Codice.Client.Common.GameUI;
using Codice.CM.SEIDInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor
{
    public class LevelRequirementsCreatorWindow : EditorWindow
    {
        private LevelRequirementSO levelRequirementSO;
        private List<LevelRequirementSO.AtomicObjectRequest> atomicObjectRequests;
        private Vector2 scrollPos;
        private int tileHeight = 100;

        private Type[] availableEventTypes;

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

            window.Show();
        }

        private void OnGUI() {
            if(levelRequirementSO == null) {
                levelRequirementSO = CreateInstance<LevelRequirementSO>();
            }

            if (atomicObjectRequests == null) {
                atomicObjectRequests = new();
            }

            if(atomicObjectRequests.Count == 0) {
                atomicObjectRequests.Add(default);
            }

            LoadAvailableEventTypes();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
            GUILayout.Label("Level Requirements Information", EditorStyles.boldLabel);

            // End Level Completion Task (right now it's a quark amount, will be a message or set of messages
            levelRequirementSO.levelCompletionTask = EditorGUILayout.LongField("Level Completion Task", levelRequirementSO.levelCompletionTask);

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
                LevelRequirementSO.AtomicObjectRequest atomicObjectRequest = atomicObjectRequests[i];

                atomicObjectRequest.atomicObjectSO = (AtomicObjectSO)EditorGUILayout.ObjectField("Atomic Object SO", atomicObjectRequest.atomicObjectSO, typeof(AtomicObjectSO), false);
                atomicObjectRequest.rewardMinimum = EditorGUILayout.LongField("Reward Minimum", atomicObjectRequest.rewardMaximum);
                atomicObjectRequest.rewardMaximum = EditorGUILayout.LongField("Reward Maximum", atomicObjectRequest.rewardMaximum);
                atomicObjectRequest.isLocked = EditorGUILayout.Toggle("Is Locked", atomicObjectRequest.isLocked);

                List<GameEventMessage> allEvents;
                if (atomicObjectRequest.unlockMessage == null) {
                    atomicObjectRequest.unlockMessage = CreateInstance<AtomicObjectRequestUnlockMessage>();
                    allEvents = new List<GameEventMessage>();
                } else {
                    allEvents = atomicObjectRequest.unlockMessage.unlockRequirements.ToList();
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Atomic Object Request Unlock Message", EditorStyles.boldLabel);
                int eventsRemoveIndex = -1;

                for(int j = 0; j < allEvents.Count; j++) {
                    var evt = allEvents[j];

                    if (evt == null) continue;

                    EditorGUILayout.BeginVertical("box");

                    EditorGUI.BeginChangeCheck();
                    evt.EditorDrawingFunction();
                    if(EditorGUI.EndChangeCheck()) {
                        EditorUtility.SetDirty(evt);
                    }

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
                if(eventsRemoveIndex >= 0) { allEvents.RemoveAt(eventsRemoveIndex); }

                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                // If the Add Message Button is pushed
                GenericMenu menu = LoadGameEventMessages(allEvents, i);
                if(EditorGUILayout.DropdownButton(new GUIContent("Add Game Message"), FocusType.Keyboard)){
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

                atomicObjectRequest.unlockMessage.unlockRequirements = allEvents.ToArray();
                atomicObjectRequests[i] = atomicObjectRequest;
                GUILayout.EndVertical();

                currentColumn++;
                if(currentColumn >= columns || i == atomicObjectRequests.Count - 1) {
                    EditorGUILayout.EndHorizontal();
                    currentColumn = 0;
                }
            }
            EditorGUILayout.EndVertical();

            if (removeIndex >= 0) { atomicObjectRequests.RemoveAt(removeIndex); }
        }

        private void LoadLevelRequirementsFromDisk() {
            throw new NotImplementedException();
        }

        private void SaveLevelFile() {
            throw new NotImplementedException();
        }

        private void LoadAvailableEventTypes() {
            availableEventTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(GameEventMessage).IsAssignableFrom(t))
                .ToArray();
        }

        private GenericMenu LoadGameEventMessages(List<GameEventMessage> allEvents, int atomicIndex) {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < availableEventTypes.Length; i++)  {
                var eventType = availableEventTypes[i];
                var data = (atomicIndex, allEvents, eventType);
                menu.AddItem(new GUIContent(eventType.Name), false, AddNewGameEventMessage, data);
            }
            return menu;
        }

        private void AddNewGameEventMessage(object userData) {
            var (atomicIndex, allEvents, messageType) = ((int, List<GameEventMessage>, Type))userData;

            GameEventMessage newEvent = CreateInstance(messageType) as GameEventMessage;
            newEvent.name = messageType.Name;

            allEvents.Add(newEvent);

            var request = atomicObjectRequests[atomicIndex];
            request.unlockMessage.unlockRequirements = allEvents.ToArray();
            atomicObjectRequests[atomicIndex] = request;

            Repaint();
        }
    }
}
