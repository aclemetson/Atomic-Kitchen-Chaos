using AtomicKitchenChaos.Messages;
using UnityEngine;

namespace AtomicKitchenChaos.Editor.MessageMappers
{
    public class EmptyMessageEditor : MessageEditor {
        public EmptyMessageEditor(GameEventMessage message) : base(message) {
        }

        public override void EditorDrawingFunction() {
            
        }
    }
}
