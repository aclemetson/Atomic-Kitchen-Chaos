using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class CombinerCompleteObjectMessageEditor : MessageEditor {

        private List<AtomicObjectSO> atomicObjectSOs;
        public CombinerCompleteObjectMessageEditor(GameEventMessage message) : base(message) {
        }

        public override void EditorDrawingFunction() {
            CombinerCompleteObjectMessage message = (CombinerCompleteObjectMessage) _message;
            if (atomicObjectSOs == null) {
                atomicObjectSOs = new();
            }

            if(message.atomicObjectSOPaths == null) {
                message.atomicObjectSOPaths = new string[atomicObjectSOs.Count];
            }

            for (int i = 0; i < message.atomicObjectSOPaths.Length; i++) { 
                if (!string.IsNullOrEmpty(message.atomicObjectSOPaths[i]) && (atomicObjectSOs.Count <= i || atomicObjectSOs[i] == null)) {
                    DataHandler.TryLoadSO(message.atomicObjectSOPaths[i], out AtomicObjectSO atomicObjectSO);
                    if (atomicObjectSOs.Count <= i) {
                        atomicObjectSOs.Add(atomicObjectSO);
                    } else {
                        atomicObjectSOs[i] = atomicObjectSO;
                    }
                } else {
                    if (atomicObjectSOs.Count <= i) {
                        atomicObjectSOs.Add(default);
                    } else {
                        atomicObjectSOs[i] = default;
                    }
                }
            }

            int removeIndex = -1;
            for (int i = 0; i < atomicObjectSOs.Count; i++) {
                atomicObjectSOs[i] = (AtomicObjectSO)EditorGUILayout.ObjectField("Atomic Object SO", atomicObjectSOs[i], typeof(AtomicObjectSO), false);
                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    removeIndex = i;
                }
            }

            if (GUILayout.Button("Add", GUILayout.Width(80))) {
                atomicObjectSOs.Add(default);
            }

            if(removeIndex >= 0) atomicObjectSOs.RemoveAt(removeIndex);

            message.atomicObjectSOPaths = atomicObjectSOs.Select(t => AssetDatabase.GetAssetPath(t)).ToArray();
            SetMessage(message);
        }
    }
}
