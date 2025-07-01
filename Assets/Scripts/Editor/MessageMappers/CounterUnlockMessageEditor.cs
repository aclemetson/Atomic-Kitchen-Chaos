using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.GeneratedObjects.ScriptableObjects;
using AtomicKitchenChaos.Messages;
using UnityEditor;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class CounterUnlockMessageEditor : MessageEditor {

        private CounterSO unlockCounterSO;

        public CounterSO UnlockCounterSO => unlockCounterSO;
        public CounterUnlockMessageEditor(GameEventMessage message) : base(message) {
        }

        public override void EditorDrawingFunction() {
            CounterUnlockMessage message = (CounterUnlockMessage)_message;

            if(!string.IsNullOrEmpty(message.counterSOPath) && unlockCounterSO == null) {
                DataHandler.TryLoadSO(message.counterSOPath, out unlockCounterSO);
            }

            unlockCounterSO = (CounterSO)EditorGUILayout.ObjectField("Counter SO", unlockCounterSO, typeof(CounterSO), false);

            message.counterSOPath = AssetDatabase.GetAssetPath(unlockCounterSO);
            SetMessage(message);
        }
    }
}
