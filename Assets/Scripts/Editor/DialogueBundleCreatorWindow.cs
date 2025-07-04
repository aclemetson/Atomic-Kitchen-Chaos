using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor
{
    public class DialogueBundleCreatorWindow : EditorWindow
    {
        private static readonly string WINDOW_NAME = "Create Dialogue";

        private string dialogueBundleName = "NewDialogueBundle";
        private DialogueBundleData dialogueBundleData;
        private string folderPath = "";
        private List<string> allDialogueFiles = new();
        private List<int> selectedIndices = new();
        private Vector2 scrollPos;

        [MenuItem("Tools/Level Tools/Create Dialogue Bundle", priority = 62)]
        public static void OpenWindow() {
            GetWindow<DialogueBundleCreatorWindow>(WINDOW_NAME);
        }

        private void OnGUI() {
            GUILayout.Space(10);

            dialogueBundleName = EditorGUILayout.TextField("Dialogue Bundle Name", dialogueBundleName);

            if(GUILayout.Button("Select Dialogue Folder")) {
                folderPath = EditorUtility.OpenFolderPanel("Select Dialogue Root", Utilities.DIR_DIALOGUE_DATA, "");
                if (!string.IsNullOrEmpty(folderPath))
                    LoadDialogueFiles();
            }

            GUILayout.Space(10);
            if (!string.IsNullOrEmpty(folderPath))
                EditorGUILayout.LabelField("Selected Folder:", folderPath);

            GUILayout.Space(10);
            DrawDialogueList();

            if(selectedIndices.Count > 0) {
                GUILayout.Space(10);
                if (GUILayout.Button("Save Dialogue Bundle")) {
                    SaveBundle();
                }
            }
        }

        private void LoadDialogueFiles() {
            allDialogueFiles.Clear();
            selectedIndices.Clear();

            var dialogueFiles = Utilities.GetFullFilePaths(folderPath);
            allDialogueFiles.AddRange(dialogueFiles);
        }

        private void DrawDialogueList() {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

            for (int i = 0; i < allDialogueFiles.Count; i++) {
                string filename = Path.GetFileName(allDialogueFiles[i]);
                bool isSelected = selectedIndices.Contains(i);

                EditorGUILayout.BeginHorizontal();

                if(GUILayout.Toggle(isSelected, filename, "Button")) {
                    if(!isSelected) selectedIndices.Add(i);
                } else {
                    selectedIndices.Remove(i);
                }

                GUI.enabled = isSelected;

                if(GUILayout.Button("▲", GUILayout.Width(30)) && selectedIndices.Contains(i) && i > 0) {
                    SwapSelected(i, i - 1);
                }

                if (GUILayout.Button("▼", GUILayout.Width(30)) && selectedIndices.Contains(i) && i < selectedIndices.Count - 1) {
                    SwapSelected(i, i + 1);
                }

                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void SwapSelected(int a, int b) {
            (selectedIndices[a], selectedIndices[b]) = (selectedIndices[b], selectedIndices[a]);
        }

        private void SaveBundle() {
            List<string> orderedFiles = new();
            foreach (int index in selectedIndices) {
                if (index >= 0 && index < allDialogueFiles.Count) {
                    orderedFiles.Add(allDialogueFiles[index]);
                }
            }

            dialogueBundleData.dialogueBundleName = dialogueBundleName;
            dialogueBundleData.dialogueDataPaths = orderedFiles.ToArray();
            string fullPath = Utilities.GetDataPath(Utilities.DIR_DIALOGUE_BUNDLE_DATA, dialogueBundleName + ".lz4");
            DataHandler.TrySaveToFile(dialogueBundleData, fullPath);
        }
    }
}
