using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using UnityEditor;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class AtomicObjectRequestSuccessMessageEditor : MessageEditor {

        private AtomicObjectSO successAtomicObjectSO;

        public AtomicObjectRequestSuccessMessageEditor(GameEventMessage message) : base(message) {
        }

        public AtomicObjectSO SuccessAtomicObjectSO => successAtomicObjectSO;

        public override void EditorDrawingFunction() {
            AtomicObjectRequestSuccessMessage message = (AtomicObjectRequestSuccessMessage)_message;

            if (!string.IsNullOrEmpty(message.atomicObjectSOPath) && successAtomicObjectSO == null) {
                DataHandler.TryLoadSO(message.atomicObjectSOPath, out successAtomicObjectSO);
            }

            successAtomicObjectSO = (AtomicObjectSO)EditorGUILayout.ObjectField("Atomic Object SO", successAtomicObjectSO, typeof(AtomicObjectSO), false);

            message.atomicObjectSOPath = AssetDatabase.GetAssetPath(successAtomicObjectSO);
            SetMessage(message);
        }
    }
}
