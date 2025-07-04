using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class AtomicFinalSubmissionRequestMessageEditor : MessageEditor {

        private AtomicObjectSO atomicObjectSO;
        public AtomicFinalSubmissionRequestMessageEditor(GameEventMessage message) : base(message) {
        }

        public override void EditorDrawingFunction() {
            AtomicFinalSubmissionRequestMessage message = (AtomicFinalSubmissionRequestMessage)_message;


            if (!string.IsNullOrEmpty(message.atomicObjectSOPath) && atomicObjectSO == null) {
                DataHandler.TryLoadSO(message.atomicObjectSOPath, out atomicObjectSO);
            }

            atomicObjectSO = (AtomicObjectSO)EditorGUILayout.ObjectField("Atomic Object SO", atomicObjectSO, typeof(AtomicObjectSO), false);
            message.quantity = EditorGUILayout.IntField("Quantity", message.quantity);

            message.atomicObjectSOPath = AssetDatabase.GetAssetPath(atomicObjectSO);
            SetMessage(message);
        }
    }
}
