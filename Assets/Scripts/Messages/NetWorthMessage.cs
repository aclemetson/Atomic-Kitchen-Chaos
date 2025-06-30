using UnityEditor;

namespace AtomicKitchenChaos.Messages
{
    public class NetWorthMessage : GameEventMessage
    {
        public long value;

        public override void EditorDrawingFunction() {
            value = EditorGUILayout.LongField("Desired Net Worth", value);
        }
    }
}
