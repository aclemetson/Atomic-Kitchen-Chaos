using AtomicKitchenChaos.Messages;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class QuarkCountMessageEditor : MessageEditor {
        public QuarkCountMessageEditor(GameEventMessage message) : base(message) {
        }

        public override void EditorDrawingFunction() {
            QuarkCountMessage message = (QuarkCountMessage)_message;

            message.quarkCount = EditorGUILayout.LongField("Quark Count", message.quarkCount);

            SetMessage(message);
        }
    }
}
