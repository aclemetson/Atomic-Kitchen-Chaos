using AtomicKitchenChaos.Messages;
using UnityEditor;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class NetWorthMessageEditor : MessageEditor {
        public NetWorthMessageEditor(GameEventMessage message) : base(message) {
        }

        public override void EditorDrawingFunction() {
            NetWorthMessage message = (NetWorthMessage)_message;
            
            message.value = EditorGUILayout.LongField("Desired Net Worth", message.value);

            SetMessage(message);
        }
    }
}
