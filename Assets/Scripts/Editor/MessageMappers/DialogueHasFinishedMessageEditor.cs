using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Messages;
using AtomicKitchenChaos.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class DialogueHasFinishedMessageEditor : MessageEditor {
        public DialogueHasFinishedMessageEditor(GameEventMessage message) : base(message) {
        }

        public override void EditorDrawingFunction() {
            DialogueHasFinishedMessage message = (DialogueHasFinishedMessage)_message;

            string assetName = string.IsNullOrEmpty(message.dialogueName) ? "None Selected" : message.dialogueName;
            EditorGUILayout.LabelField(assetName, GUILayout.MaxWidth(200));

            if (GUILayout.Button("Browse", GUILayout.Width(80))) {
                string selectedPath = EditorUtility.OpenFilePanel(
                    "Select Dialogue Asset",
                    Utilities.DIR_DIALOGUE_DATA,
                    "lz4"
                    );
                if (!string.IsNullOrEmpty(selectedPath)) {
                    // Convert full path to relative project path
                    var path = Utilities.GetUnityRelativeAssetPath(selectedPath);
                    if (!DataHandler.TryLoadFromFile(path, out DialogueHasFinishedMessage objData)) {
                        Debug.LogError("Selected file is not valid");
                        return;
                    }
                    message.dialogueName = Path.GetFileNameWithoutExtension(path);
                }
            }

            SetMessage(message);
        }
    }
}
